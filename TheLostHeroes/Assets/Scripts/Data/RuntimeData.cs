using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using System;
using System.Text;

public struct RuntimeData {
    //stuff only created in runtime
    
    public EcsEntity envPlayer;
    public EcsEntity Map;

    public class RandomConfiguration {
        private Random randomDevice;
        readonly string seed;

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        public RandomConfiguration() {
            randomDevice = new Random();

            var seedBuilder = new StringBuilder();
            var len = randomDevice.Next(3, 79);
            for (var i = 0; i < len; i++) {
                seedBuilder.Append(chars[randomDevice.Next(chars.Length)]);
            }

            seed = seedBuilder.ToString();
            randomDevice = new Random(seed.GetHashCode());
        }

        public RandomConfiguration(string seed) {
            randomDevice = new Random(seed.GetHashCode());
            this.seed = string.Copy(seed);
        }
    }

    public RandomConfiguration randomConfiguration;
}
