using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;
using System;

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

            var mapDict = new Dictionary<Vector2Int, BlockType> {
                { Vector2Int.zero, BlockType.Hall }
            };

            var diggers = new List<Digger>{
                new HallDigger(mapDict, runtimeData.randomConfiguration, Vector2Int.zero, 0)
            };

            int numEpochs = 10;
            float spawnRate = 0.2f;
            float deathRate = 0.105f;

            for (var epoch = 0; epoch < numEpochs; epoch++) {
                var removeIndices = new List<int>();

                for (var diggeridx = 0; diggeridx < diggers.Count; diggeridx++) {
                    var prevdir = diggers[diggeridx].dir;
                    var revprevdir = (prevdir + 2) % 4;
                    diggers[diggeridx].Step();
                    var currdir = diggers[diggeridx].dir;
                    var currposition = diggers[diggeridx].position;
                    
                    for (var dir = 0; dir < 4; dir++) {
                        // добавить создание комнаты
                        
                        if (dir == currdir) continue;
                        if (dir == revprevdir) continue;

                        if (!runtimeData.randomConfiguration.Binrary(spawnRate)) continue;

                        diggers.Add(
                            new HallDigger(mapDict, runtimeData.randomConfiguration, currposition, (uint)dir)
                        );
                    }

                    if (!runtimeData.randomConfiguration.Binrary(deathRate)) continue;
                    diggers[diggeridx].Kill();

                    if (!diggers[diggeridx].Alive()) removeIndices.Add(diggeridx);
                }

                for (var i = 0; i < removeIndices.Count; i++) {
                    diggers.RemoveAt(removeIndices[i] - i);
                }
            }

            int xmin = int.MaxValue;
            int ymin = int.MaxValue;
            int xmax = int.MinValue;
            int ymax = int.MinValue;

            foreach (var elem in mapDict) {
                xmin = Math.Min(xmin, elem.Key.x);
                ymin = Math.Min(ymin, elem.Key.y);
                xmax = Math.Max(xmax, elem.Key.x);
                ymax = Math.Max(ymax, elem.Key.y);
            }

            var texture = new Texture2D(xmax - xmin + 1, ymax - ymin + 1) {
                filterMode = FilterMode.Point
            };

            foreach (var elem in mapDict) {
                texture.SetPixel(elem.Key.x - xmin, elem.Key.y - ymin, Color.red);
                //  elem.Value;
                texture.Apply();
            }
    
            /************************** Эксперименты все **************************/

            float pixelsPerUnit = 50;

            mapRenderer.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(),
                //new Vector2(- 0.5f / pixelsPerUnit * texture.width, -0.5f / pixelsPerUnit *texture.height),
                pixelsPerUnit
            );
            
            var mapTransform = mapRenderer.gameObject.GetComponent<Transform>();
            mapTransform.position -= 0.5f / pixelsPerUnit * new Vector3(texture.width, texture.height);
        }
    }

    private abstract class Digger {
        public static readonly Vector2Int[] directions = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        
        public uint dir = 0;
        public Vector2Int position;

        protected bool alive = true;
        
        protected RandomConfiguration randomDevice = null;
        protected Dictionary<Vector2Int, BlockType> map;

        public abstract bool Step ();

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
        }

        public override bool Step () {
            if (!alive) return alive;

            if (!map.ContainsKey(position)) throw new Exception();

            var len = Convert.ToInt32(randomDevice.NextNormal(8, 1.5));
            len = Math.Max(len, 3);
            len = Math.Min(len, 17);

            for (var i = 0; i < len; i++) {
                position += directions[dir];

                if (map.ContainsKey(position)) {
                    if (map[position] == BlockType.Wall) {
                        if (map[position + directions[dir]] == BlockType.Room) {
                            map[position] = BlockType.Door;
                        }
                    }

                    alive = false;
                    break;
                }

                map.Add(position, BlockType.Hall);
            }

            if (randomDevice.Binrary(0.4)) {
                if (randomDevice.Binrary(0.5)) {
                    dir += 1;
                } else {
                    dir += 3;
                }

                dir %= 4;
            }

            return alive;
        }
    }

    private class RoomDigger : Digger {
        public RoomDigger (Dictionary<Vector2Int, BlockType> map, RandomConfiguration randomDevice, Vector2Int position, uint dir) {
            alive = false;

            this.dir = dir % 4;
            this.position = position;
            this.randomDevice = randomDevice;
            this.map = map;
        }

        private void CreateRoom () {
            var roomForward = directions[dir];
            var roomRight = directions[(dir + 3) % 4];
        
            var roomWidth = Convert.ToInt32(randomDevice.NextNormal(6, 2));
            var roomDepth = Convert.ToInt32(randomDevice.NextNormal(6, 2));
        
            roomWidth = Math.Min(Math.Max(3, roomWidth), 12);
            roomDepth = Math.Min(Math.Max(3, roomDepth), 12);

            var offset = randomDevice.Next(roomWidth);

            for (var x = -offset; x < roomWidth - offset; x++) {
                for (var y = 0; y < roomDepth; y++) {
                    var currpos = x * roomRight + y * roomForward + position + 2 * directions[dir];

                    if (map.ContainsKey(currpos)) {
                        return;
                    }
                }
            }

            for (var x = -offset; x < roomWidth - offset; x++) {
                var currpos = x * roomRight + (-1) * roomForward + position + 2 * directions[dir];
                
                if (map.ContainsKey(currpos) && map[currpos] != BlockType.Wall) {
                    return;
                }
            }
            for (var x = -offset; x < roomWidth - offset; x++) {
                var currpos = x * roomRight + roomDepth * roomForward + position + 2 * directions[dir];
                
                if (map.ContainsKey(currpos) && map[currpos] != BlockType.Wall) {
                    return;
                }
            }
            for (var y = 0; y < roomDepth; y++) {
                var currpos = (-offset - 1) * roomRight + y * roomForward + position + 2 * directions[dir];

                if (map.ContainsKey(currpos) && map[currpos] != BlockType.Wall) {
                    return;
                }
            }
            for (var y = 0; y < roomDepth; y++) {
                var currpos = (roomWidth-offset) * roomRight + y * roomForward + position + 2 * directions[dir];

                if (map.ContainsKey(currpos) && map[currpos] != BlockType.Wall) {
                    return;
                }
            }

            for (var x = -offset; x < roomWidth - offset; x++) {
                for (var y = 0; y < roomDepth; y++) {
                    var currpos = x * roomRight + y * roomForward + position + 2 * directions[dir];

                    if (map.ContainsKey(currpos)) {
                        return;
                    }
                }
            }

            for (var x = -offset; x < roomWidth - offset; x++) {
                var currpos = x * roomRight + (-1) * roomForward + position + 2 * directions[dir];
                
                map.Add(currpos, BlockType.Room);
            }
            for (var x = -offset; x < roomWidth - offset; x++) {
                var currpos = x * roomRight + roomDepth * roomForward + position + 2 * directions[dir];

                map.Add(currpos, BlockType.Wall);
            }
            for (var y = 0; y < roomDepth; y++) {
                var currpos = (- offset - 1) * roomRight + y * roomForward + position + 2 * directions[dir];

                map.Add(currpos, BlockType.Wall);
            }
            for (var y = 0; y < roomDepth; y++) {
                var currpos = (roomWidth - offset) * roomRight + y * roomForward + position + 2 * directions[dir];

                map.Add(currpos, BlockType.Wall);
            }
        }

        public override bool Step () {
            return alive;
        }
    }
}
