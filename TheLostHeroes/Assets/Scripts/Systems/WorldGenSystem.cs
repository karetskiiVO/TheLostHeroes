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

    public void Init() {
        foreach (var i in filter) {
            ref var mapEntity = ref filter.GetEntity(i);
            ref var mapComponent = ref filter.Get1(i);
            var mapRenderer = mapComponent.renderer;

            /******************* Некоторые эксперименты на тему *******************/

            var mapDict = new Dictionary<Vector2Int, uint>();
            
            var diggers = new List<Digger>{
                new HallDigger(mapDict, runtimeData.randomConfiguration)
            };

            int numEpochs = 10;
            float spawnRate = 0.2f;
            float deathRate = 0.105f;

            for (var epoch = 0; epoch < numEpochs; epoch++) {
                for (var diggeridx = 0; diggeridx < diggers.Count; diggeridx++) {
                    diggers[diggeridx].Step();

                    for (var dir = 0; dir < 4; dir++) {
                        // добавить создание комнаты
                        
                        if ((dir + diggers[diggeridx].dir) % 2 == 0) continue;
                        if (!runtimeData.randomConfiguration.Binrary(spawnRate)) continue;

                        diggers.Add(new HallDigger(mapDict, runtimeData.randomConfiguration, diggers[diggeridx].position, dir));
                    }

                    if (!runtimeData.randomConfiguration.Binrary(deathRate)) continue;
                    diggers[diggeridx].Kill();
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

            var texture = new Texture2D(xmax - xmin + 1, ymax - ymin + 1);

            foreach (var elem in mapDict) {
                texture.SetPixel(elem.Key.x - xmin, elem.Key.y - ymin, Color.red);
                //  elem.Value;
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();
    
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
        protected Dictionary<Vector2Int, uint> map;

        public abstract bool Step ();

        public bool Alive () {
            return alive;
        }
        public void Kill () {
            alive = false;
        }
    }

    private class HallDigger : Digger {
        public HallDigger (Dictionary<Vector2Int, uint> map, RandomConfiguration randomDevice, Vector2Int position = default, int dir = -1) {
            this.randomDevice = randomDevice;

            this.map = map;

            if (dir < 0 || dir > 4) {
                dir = randomDevice.Next(4);
            }
            this.dir = (uint)dir;
            this.position = position;
        }

        public override bool Step () {
            if (!alive) return alive;

            var len = Convert.ToInt32(randomDevice.NextNormal(8, 1.5));
            len = Math.Max(len, 3);

            for (var i = 0; i < len; i++) {
                if (!map.ContainsKey(position)) {
                    map[position] = 1;
                }
                position += directions[dir];

                if (map.ContainsKey(position)) {
                    // переделать под комнаты
                    alive = false;
                    break;
                }

                map[position] = 1;
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
        public override bool Step () {
            throw new NotImplementedException();
        }
    }
}
