using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;
using System;
using Unity.VisualScripting;

public struct WorldGenSystem : IEcsInitSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    public EcsFilter<Map> filter;

    private enum BlockType : uint {
        Hall = 1,
        Room = 2,
        Wall = 3,
        Door = 4,
    }

    public void Init() {
        foreach (var filteridx in filter) {
            ref var mapEntity = ref filter.GetEntity(filteridx);
            ref var mapComponent = ref filter.Get1(filteridx);
            var mapRenderer = mapComponent.renderer;

            /******************* Некоторые эксперименты на тему *******************/

            var dungeonAccumulator = new DungeonAccumulator(runtimeData.randomConfiguration);
            dungeonAccumulator.Genere();
            dungeonAccumulator.Clear();
            var texture = dungeonAccumulator.DebugVisualise();
    
            /************************** Эксперименты все **************************/

            float pixelsPerUnit = 50;

            mapRenderer.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(),
                pixelsPerUnit
            );
            
            var mapTransform = mapRenderer.gameObject.GetComponent<Transform>();
            mapTransform.position -= 0.5f / pixelsPerUnit * new Vector3(texture.width, texture.height);
        }
    }

    private class DungeonAccumulator {
        private Dictionary<Vector2Int, BlockType> map;
        private readonly RandomConfiguration randomDevice;

        public DungeonAccumulator(RandomConfiguration randomDevice) {
            this.randomDevice = randomDevice;
        }

        public void Genere (int numEpochs = 10, float spawnRate = 0.7f, float deathRate = 0.05f, float roomSpawnRate = 0.6f) {
            map = new Dictionary<Vector2Int, BlockType> {
                { Vector2Int.zero, BlockType.Hall }
            };

            var diggers = new List<Digger>{
                NewHallDigger(Vector2Int.zero, 0)
            };

            for (var epoch = 0; epoch < numEpochs; epoch++) {
                var removeIndices = new List<int>();

                for (var diggeridx = 0; diggeridx < diggers.Count; diggeridx++) {
                    diggers[diggeridx].Step();
                    var currposition = diggers[diggeridx].position;
                    
                    for (uint dir = 0; dir < 4; dir++) {
                        if (!diggers[diggeridx].SpawnNew(spawnRate, dir, out Vector2Int newpos)) continue;

                        if (randomDevice.Binrary(roomSpawnRate)) {
                            diggers.Add(NewRoomDigger(newpos, dir));
                        } else {
                            diggers.Add(NewHallDigger(newpos, dir));
                        }
                    }

                    if (Math.Max(Math.Abs(currposition.x), Math.Abs(currposition.y)) >= 256) {
                        diggers[diggeridx].Kill();
                    } else {
                        diggers[diggeridx].ProbabilityKill(deathRate);
                    }


                    if (!diggers[diggeridx].Alive()) removeIndices.Add(diggeridx);
                }

                for (var i = 0; i < removeIndices.Count; i++) {
                    diggers.RemoveAt(removeIndices[i] - i);
                }
            }
        }

        public void Clear () {
            Debug.Log("DungeonAccumulator.Clear says: \"implement me\"");
        }

        public Texture2D DebugVisualise () {
            int xmin = int.MaxValue;
            int ymin = int.MaxValue;
            int xmax = int.MinValue;
            int ymax = int.MinValue;

            foreach (var elem in map) {
                xmin = Math.Min(xmin, elem.Key.x);
                ymin = Math.Min(ymin, elem.Key.y);
                xmax = Math.Max(xmax, elem.Key.x);
                ymax = Math.Max(ymax, elem.Key.y);
            }

            var texture = new Texture2D(xmax - xmin + 1, ymax - ymin + 1) {
                filterMode = FilterMode.Point
            };

            var visualisationDict = new Dictionary<BlockType, Color>{
                { BlockType.Door, Color.red    },
                { BlockType.Hall, Color.cyan   },
                { BlockType.Wall, Color.green  },
                { BlockType.Room, Color.yellow },
            };

            foreach (var elem in map) {
                texture.SetPixel(elem.Key.x - xmin, elem.Key.y - ymin, visualisationDict[elem.Value]);
                texture.Apply();
            }

            return texture;
        }

        public HallDigger NewHallDigger (Vector2Int position, uint dir) {
            return new HallDigger(map, randomDevice, position, dir);
        }

        public RoomDigger NewRoomDigger (Vector2Int position, uint dir) {
            return new RoomDigger(map, randomDevice, position, dir);
        }
    }

    private abstract class Digger {
        public static readonly Vector2Int[] directions = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        
        protected uint prevdir = 10;
        public uint dir = 0;
        public Vector2Int position;

        protected bool alive = true;
        
        protected RandomConfiguration randomDevice = null;
        protected Dictionary<Vector2Int, BlockType> map;

        public abstract bool Step ();
        public abstract bool SpawnNew (float spawnrate, uint newdir, out Vector2Int newpos);
        public abstract bool ProbabilityKill (float deathrate);

        public bool Alive () {
            return alive;
        }
        public void Kill () {
            alive = false;
        }
    }

    private class HallDigger : Digger {
        public HallDigger (Dictionary<Vector2Int, BlockType> map, RandomConfiguration randomDevice, Vector2Int position, uint dir) {
            this.randomDevice = randomDevice;

            this.map = map;

            this.dir = dir % 4;
            this.position = position;
            
            prevdir = dir;
        }

        public override bool SpawnNew(float spawnrate, uint newdir, out Vector2Int newpos) {
            newpos = default;
            if (newdir == dir || newdir == (prevdir + 2) % 4) {
                return false;
            }

            return randomDevice.Binrary(spawnrate);
        }

        public override bool Step () {
            if (!alive) return alive;

            var len = Convert.ToInt32(randomDevice.NextNormal(6, 2));
            len = Math.Max(len, 3);
            len = Math.Min(len, 15);

            if (map.ContainsKey(position) && map[position] == BlockType.Wall) {
                map[position] = BlockType.Door;
            }

            for (var i = 0; i < len; i++) {
                position += directions[dir];

                if (map.ContainsKey(position)) {
                    if (map[position] == BlockType.Wall) {
                        if (map.ContainsKey(position + directions[dir]) && map[position + directions[dir]] == BlockType.Room) {
                            map[position] = BlockType.Door;
                        }
                    }

                    alive = false;
                    break;
                }

                map.Add(position, BlockType.Hall);
            }

            if (randomDevice.Binrary(0.4)) {
                prevdir = dir;

                if (randomDevice.Binrary(0.5)) {
                    dir += 1;
                } else {
                    dir += 3;
                }

                dir %= 4;
            }

            return alive;
        }
    
        public override bool ProbabilityKill (float deathrate) {
            if (randomDevice.Binrary(deathrate)) {
                alive = false;
            }

            return alive;
        }
    }

    private class RoomDigger : Digger {
        public RoomDigger (Dictionary<Vector2Int, BlockType> map, RandomConfiguration randomDevice, Vector2Int position, uint dir) {
            this.dir = dir % 4;
            this.position = position;
            this.randomDevice = randomDevice;
            this.map = map;

            prevdir = dir;

            alive = CreateRoom();
        }

        int roomWidth;
        int roomDepth;
        int offset;

        private bool CreateRoom () {
            var roomForward = directions[dir];
            var roomRight = directions[(dir + 3) % 4];
        
            roomWidth = Convert.ToInt32(randomDevice.NextNormal(6, 2));
            roomDepth = Convert.ToInt32(randomDevice.NextNormal(6, 2));
        
            roomWidth = Math.Min(Math.Max(3, roomWidth), 12);
            roomDepth = Math.Min(Math.Max(3, roomDepth), 12);

            offset = randomDevice.Next(roomWidth);

            if (map.ContainsKey(position) && map[position] == BlockType.Wall) {
                position -= roomForward;
            }

            for (var x = -offset; x < roomWidth - offset; x++) {
                for (var y = 0; y < roomDepth; y++) {
                    var currpos = x * roomRight + y * roomForward + position + 2 * directions[dir];

                    if (map.ContainsKey(currpos)) {
                        return false;
                    }
                }
            }

            for (var x = -offset; x < roomWidth - offset; x++) {
                var currpos = x * roomRight - roomForward + position + 2 * roomForward;
                
                if (map.ContainsKey(currpos) && map[currpos] != BlockType.Wall) {
                    return false;
                }
            }
            for (var x = -offset; x < roomWidth - offset; x++) {
                var currpos = x * roomRight + roomDepth * roomForward + position + 2 * roomForward;
                
                if (map.ContainsKey(currpos) && map[currpos] != BlockType.Wall) {
                    return false;
                }
            }
            for (var y = 0; y < roomDepth; y++) {
                var currpos = (-offset - 1) * roomRight + y * roomForward + position + 2 * roomForward;

                if (map.ContainsKey(currpos) && map[currpos] != BlockType.Wall) {
                    return false;
                }
            }
            for (var y = 0; y < roomDepth; y++) {
                var currpos = (roomWidth-offset) * roomRight + y * roomForward + position + 2 * roomForward;

                if (map.ContainsKey(currpos) && map[currpos] != BlockType.Wall) {
                    return false;
                }
            }

            for (var x = -offset; x < roomWidth - offset; x++) {
                for (var y = 0; y < roomDepth; y++) {
                    var currpos = x * roomRight + y * roomForward + position + 2 * roomForward;

                    map[currpos] = BlockType.Room;
                }
            }

            for (var x = -offset; x < roomWidth - offset; x++) {
                var currpos = x * roomRight + (-1) * roomForward + position + 2 * roomForward;
                
                map[currpos] = BlockType.Wall;
            }
            for (var x = -offset; x < roomWidth - offset; x++) {
                var currpos = x * roomRight + roomDepth * roomForward + position + 2 * roomForward;

                map[currpos] = BlockType.Wall;
            }
            for (var y = 0; y < roomDepth; y++) {
                var currpos = (- offset - 1) * roomRight + y * roomForward + position + 2 * roomForward;

                map[currpos] = BlockType.Wall;
            }
            for (var y = 0; y < roomDepth; y++) {
                var currpos = (roomWidth - offset) * roomRight + y * roomForward + position + 2 * roomForward;

                map[currpos] = BlockType.Wall;
            }

            map[position + roomForward] = BlockType.Door;
            return true;
        }

        public override bool Step () {
            return alive;
        }

        public override bool SpawnNew(float spawnrate, uint newdir, out Vector2Int newpos) {
            newpos = default;
            if (!alive) return false;
            
            var roomForward = directions[dir];
            var roomRight = directions[(dir + 3) % 4];

            if (newdir == (prevdir + 2) % 4) {
                return false;
            }

            if (newdir == dir) {
                newpos = position + (2 + roomDepth) * roomForward + (randomDevice.Next(roomWidth) - offset) * roomRight;
                return randomDevice.Binrary(spawnrate);
            }
            
            newpos = position + (2 + randomDevice.Next(roomDepth)) * roomForward;

            if (newdir == (dir + 1) % 4) {
                newpos += (- offset - 1) * roomRight;
            } if (newdir == (dir + 3) % 4) {
                newpos += (- offset + roomWidth) * roomRight;
            }

            return randomDevice.Binrary(spawnrate);
        }

        public override bool ProbabilityKill (float deathrate) {
            alive = false;

            return alive;
        }
    }
}
