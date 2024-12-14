using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public struct WorldGenSystem : IEcsInitSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    public EcsFilter<Map> filter;

    private enum BlockType : uint {
        WithFloor = 1,
        Hall = (1 << 1) | WithFloor,
        Room = (1 << 2) | WithFloor,
        Door = (1 << 3) | WithFloor,
        Wall = 1 << 4,
    }

    private struct RoomInfo {
        public int xmin;
        public int ymin;
        public int xmax;
        public int ymax;
    }

    public readonly void Init() {
        foreach (var filteridx in filter) {
            ref var mapEntity = ref filter.GetEntity(filteridx);
            ref var mapComponent = ref filter.Get1(filteridx);
            var mapRenderer = mapComponent.renderer;

            /******************* Некоторые эксперименты на тему *******************/

            var dungeonAccumulator = new DungeonAccumulator(
                runtimeData.randomConfiguration,
                mapComponent.tilemap,
                mapComponent.renderer,
                mapComponent.sprites
            );
            dungeonAccumulator.Genere();
            dungeonAccumulator.Clear();
            dungeonAccumulator.Bake();

            // В этот момент можно вытащить информацию о комнатах
        
            /************************** Эксперименты все **************************/
        }
    }

    private class DungeonAccumulator {
        private Dictionary<Vector2Int, BlockType> map;
        private readonly RandomConfiguration randomDevice;
        private List<RoomInfo> roomsInfo = new List<RoomInfo>();
        
        private Tilemap tilemap;
        private TilemapRenderer renderer;
        private SpriteAtlas sprites;

        public DungeonAccumulator(RandomConfiguration randomDevice, Tilemap tilemap, TilemapRenderer renderer, SpriteAtlas sprites) {
            this.randomDevice = randomDevice;

            this.tilemap = tilemap;
            this.renderer = renderer;
            this.sprites = sprites;
        }

        public void Genere (int numEpochs = 10, float spawnRate = 0.7f, float deathRate = 0.05f, float roomSpawnRate = 0.6f, int minroomsCnt = 7) {
            while (roomsInfo.Count < minroomsCnt) {
                roomsInfo.Clear();

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

                        if (Math.Max(Math.Abs(currposition.x), Math.Abs(currposition.y)) >= 128) {
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
        }

        public void Clear () {
            Debug.Log("DungeonAccumulator.Clear says: \"implement me\"");
        }

        private void AddWalls () {
            List<Vector2Int> neightboursCoords = new List<Vector2Int>{
                Vector2Int.left + Vector2Int.up  , Vector2Int.zero + Vector2Int.up  , Vector2Int.right + Vector2Int.up  ,
                Vector2Int.left + Vector2Int.zero,                                    Vector2Int.right + Vector2Int.zero,
                Vector2Int.left + Vector2Int.down, Vector2Int.zero + Vector2Int.down, Vector2Int.right + Vector2Int.down,
            };
            
            var cellsCoords = new List<Vector2Int>(map.Keys);

            foreach (var coord in cellsCoords) {
                if (map[coord] == BlockType.Wall) continue;

                foreach (var neightboursCoord in neightboursCoords) {
                    if (!map.ContainsKey(coord + neightboursCoord)) {
                        map[coord + neightboursCoord] = BlockType.Wall;
                    }
                }
            }
        }

        public void DebugBake () {
            AddWalls();
            
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
            }

            texture.Apply();
            // return texture;
        }

        public void Bake () {
            // // Все это слишком неэффективно и надо бы переделать

            AddWalls();

            var xcent = 0;
            var ycent = 0;

            foreach (var elem in map) {
                xcent += elem.Key.x;
                ycent += elem.Key.y;
            }

            xcent /= map.Count;
            ycent /= map.Count;

            var pivot = new Vector2Int(xcent, ycent);
            var quadredMap = new Dictionary<Vector2Int, BlockType>();

            var usedSpritesIndeces = new HashSet<int>{
                27, 28, 29, 4, 5, 6, 49, 50, 51, 46, 24, 1, 131, 112, 90, 133, 114, 92, 69, 68, 70, 93, 94, 134, 135
            };

            var loadedSprites = new Dictionary<int, Sprite>();
            foreach (var idx in usedSpritesIndeces) {
                loadedSprites[idx] = sprites.GetSprite("Tiles_" + idx.ToString());
            }

            foreach (var elem in map) {
                for (var dx = 0; dx < 4; dx++) {
                    for (var dy = 0; dy < 4; dy++) {
                        quadredMap.Add(4 * (elem.Key - pivot) + new Vector2Int(dx, dy), elem.Value);
                    }
                }
            }

            var neighbours = new Vector2Int[] {
                Vector2Int.up + Vector2Int.left, Vector2Int.up, Vector2Int.right + Vector2Int.up, Vector2Int.right,
                Vector2Int.right + Vector2Int.down, Vector2Int.down, Vector2Int.left + Vector2Int.down, Vector2Int.left
            };

            var floorSprites = new Dictionary<int, Sprite> {
                { 0b1100_0001, loadedSprites[27] },
                { 0b1000_0001, loadedSprites[27] },
                { 0b1100_0000, loadedSprites[27] },
                { 0b0000_0000, loadedSprites[28] },
                { 0b0001_1100, loadedSprites[29] },
                { 0b0000_1100, loadedSprites[29] },
                { 0b0001_1000, loadedSprites[29] },
                { 0b1100_0111, loadedSprites[4]  },
                { 0b1100_0110, loadedSprites[4]  },
                { 0b0000_0111, loadedSprites[5]  },
                { 0b0000_0011, loadedSprites[5]  },
                { 0b0000_0110, loadedSprites[5]  },
                { 0b0001_1111, loadedSprites[6]  },
                { 0b0001_1011, loadedSprites[6]  },
                { 0b1111_0001, loadedSprites[49] },
                { 0b1011_0001, loadedSprites[49] },
                { 0b0111_0000, loadedSprites[50] },
                { 0b0011_0000, loadedSprites[50] },
                { 0b0110_0000, loadedSprites[50] },
                { 0b0111_1100, loadedSprites[51] },
                { 0b0110_1100, loadedSprites[51] },
            };

            var wallSprites = new Dictionary<int, Sprite[]> {
                { 0b1111_1111, new Sprite[] {} },
                { 0b1000_1111, new Sprite[] {loadedSprites[46] , loadedSprites[24] , loadedSprites[1]}},
                { 0b1100_1111, new Sprite[] {loadedSprites[46] , loadedSprites[24] , loadedSprites[1]}},
                { 0b1001_1111, new Sprite[] {loadedSprites[46] , loadedSprites[24] , loadedSprites[1]}},
                { 0b0000_1110, new Sprite[] {loadedSprites[131], loadedSprites[112], loadedSprites[90]}},
                { 0b0100_1110, new Sprite[] {loadedSprites[131], loadedSprites[112], loadedSprites[90]}},
                { 0b1000_0011, new Sprite[] {loadedSprites[133], loadedSprites[114], loadedSprites[92]}},
                { 0b1001_0011, new Sprite[] {loadedSprites[133], loadedSprites[114], loadedSprites[92]}},
                { 0b1111_1000, new Sprite[] {loadedSprites[69]}},
                { 0b1111_1001, new Sprite[] {loadedSprites[69]}},
                { 0b1111_1100, new Sprite[] {loadedSprites[69]}},
                { 0b0011_1000, new Sprite[] {loadedSprites[68]}},
                { 0b0011_1001, new Sprite[] {loadedSprites[68]}},
                { 0b1110_0000, new Sprite[] {loadedSprites[70]}},
                { 0b1110_0100, new Sprite[] {loadedSprites[70]}},
                { 0b1110_1111, new Sprite[] {null, null, loadedSprites[93]}},
                { 0b1011_1111, new Sprite[] {null, null, loadedSprites[94]}},
                { 0b1111_1011, new Sprite[] {loadedSprites[134]}},
                { 0b1111_1110, new Sprite[] {loadedSprites[135]}},
            };

            var lVerticalStartsMasks   = new HashSet<int>{/*90, 135*/ 0b0000_1110, 0b0100_1110, 0b1111_1110};
            var rVerticalStartsMasks   = new HashSet<int>{/*92, 134*/ 0b1000_0011, 0b1001_0011, 0b1111_1011};
            var lVerticalFinishesMasks = new HashSet<int>{/*68, 94 */ 0b0011_1000, 0b0011_1001, 0b1011_1111};
            var rVerticalFinishesMasks = new HashSet<int>{/*70, 93 */ 0b1110_0000, 0b1110_0100, 0b1110_1111};

            var lVerticalStarts = new HashSet<Vector2Int>();
            var rVerticalStarts = new HashSet<Vector2Int>();
            var lVerticalFinishes = new HashSet<Vector2Int>();
            var rVerticalFinishes = new HashSet<Vector2Int>();

            foreach (var elem in quadredMap) {
                if ((elem.Value & BlockType.WithFloor) != 0) {
                    int floormask = 0;

                    for (int neightbourCnt = 0; neightbourCnt < neighbours.Length; neightbourCnt++) {
                        if (!quadredMap.ContainsKey(elem.Key + neighbours[neightbourCnt])) {
                            floormask |= 1 << neightbourCnt;
                        } else if ((quadredMap[elem.Key + neighbours[neightbourCnt]] & BlockType.WithFloor) == 0) {
                            floormask |= 1 << neightbourCnt;
                        }
                    }

                    Tile tile = ScriptableObject.CreateInstance<Tile>();
                    if (!floorSprites.ContainsKey(floormask)) {
                        floormask = 0;
                    }
                    tile.sprite = floorSprites[floormask];

                    tile.transform = Matrix4x4.TRS(new Vector3(elem.Key.x, elem.Key.y, 0) / 9, Quaternion.identity, Vector3.one / 9);
                    tilemap.SetTile(new Vector3Int(elem.Key.x, elem.Key.y, 0), tile);
                }

                if (elem.Value == BlockType.Wall) {
                    int wallmask = 0;

                    for (int neightbourCnt = 0; neightbourCnt < neighbours.Length; neightbourCnt++) {
                        if (!quadredMap.ContainsKey(elem.Key + neighbours[neightbourCnt]))  {
                            wallmask |= 1 << neightbourCnt;
                        } else if (quadredMap[elem.Key + neighbours[neightbourCnt]] == BlockType.Wall) {
                            wallmask |= 1 << neightbourCnt;
                        }
                    }

                    if (!wallSprites.ContainsKey(wallmask)) {
                        wallmask = 0b1111_1111;
                    }
                    var currWallSprites = wallSprites[wallmask];

                    if (lVerticalStartsMasks.Contains(wallmask)) 
                        lVerticalStarts.Add(elem.Key + new Vector2Int(0, wallSprites[wallmask].Length - 1));
                    else if (rVerticalStartsMasks.Contains(wallmask)) 
                        rVerticalStarts.Add(elem.Key + new Vector2Int(0, wallSprites[wallmask].Length - 1));
                    else if (lVerticalFinishesMasks.Contains(wallmask)) 
                        lVerticalFinishes.Add(elem.Key + new Vector2Int(0, wallSprites[wallmask].Length - 1));
                    else if (rVerticalFinishesMasks.Contains(wallmask)) 
                        rVerticalFinishes.Add(elem.Key + new Vector2Int(0, wallSprites[wallmask].Length - 1));

                    for (var i = 0; i < currWallSprites.Length; i++) {
                        if (currWallSprites[i] == null) continue;

                        Tile tile = ScriptableObject.CreateInstance<Tile>();
                        tile.transform = Matrix4x4.TRS(new Vector3(elem.Key.x, elem.Key.y + i, 0) / 9, Quaternion.identity, Vector3.one / 9);
                        tile.sprite = currWallSprites[i];

                        tilemap.SetTile(new Vector3Int(elem.Key.x, elem.Key.y + i, 0), tile);
                    }
                }
            }

            var leftWallSprite = sprites.GetSprite("Tiles_116");
            foreach (var lstart in lVerticalStarts) {
                var pos = lstart + Vector2Int.up;

                while (!lVerticalFinishes.Contains(pos)) {
                    Tile tile = ScriptableObject.CreateInstance<Tile>();
                    tile.transform = Matrix4x4.TRS(new Vector3(pos.x, pos.y, 0) / 9, Quaternion.identity, Vector3.one / 9);
                    tile.sprite = leftWallSprite;

                    tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);

                    pos += Vector2Int.up;
                }
            }

            var rightWallSprite = sprites.GetSprite("Tiles_115");
            foreach (var rstart in rVerticalStarts) {
                var pos = rstart + Vector2Int.up;

                while (!rVerticalFinishes.Contains(pos)) {
                    Tile tile = ScriptableObject.CreateInstance<Tile>();
                    tile.transform = Matrix4x4.TRS(new Vector3(pos.x, pos.y, 0) / 9, Quaternion.identity, Vector3.one / 9);
                    tile.sprite = rightWallSprite;

                    tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);

                    pos += Vector2Int.up;
                }
            }
            
            tilemap.RefreshAllTiles();
        }

        public HallDigger NewHallDigger (Vector2Int position, uint dir) {
            return new HallDigger(map, randomDevice, position, dir);
        }

        public RoomDigger NewRoomDigger (Vector2Int position, uint dir) {
            return new RoomDigger(map, randomDevice, position, dir, roomsInfo);
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
        private int roomWidth;
        private int roomDepth;
        private int offset;
        private RoomInfo roomInfo;

        public RoomDigger (Dictionary<Vector2Int, BlockType> map, RandomConfiguration randomDevice, Vector2Int position, uint dir, List<RoomInfo> roomInfos) {
            this.dir = dir % 4;
            this.position = position;
            this.randomDevice = randomDevice;
            this.map = map;

            prevdir = dir;

            roomInfo.xmin = int.MaxValue;
            roomInfo.xmax = int.MinValue;
            roomInfo.ymin = int.MaxValue;
            roomInfo.ymax = int.MinValue;

            alive = CreateRoom();
            if (alive) {
                roomInfos.Add(roomInfo);
            }
        }

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

                    // Оптимизировать бы по-хорошему

                    roomInfo.xmax = Math.Max(roomInfo.xmax, currpos.x);
                    roomInfo.xmin = Math.Min(roomInfo.xmin, currpos.x);
                    roomInfo.ymax = Math.Max(roomInfo.ymax, currpos.y);
                    roomInfo.ymin = Math.Min(roomInfo.ymin, currpos.y);
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
