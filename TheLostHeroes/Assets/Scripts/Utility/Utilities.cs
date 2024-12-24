using System;
using System.Text;

public class PlayerMoney {
    public int money = 0;
}

public class RandomConfiguration {
    private readonly Random randomDevice;
    readonly string seed;

    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    
    public RandomConfiguration() {
        randomDevice = new Random();

        var seedBuilder = new StringBuilder();
        var len = randomDevice.Next(3, 70);
        for (var i = 0; i < len; i++) {
            seedBuilder.Append(chars[randomDevice.Next(chars.Length)]);
        }

        seed = seedBuilder.ToString();
        randomDevice = new Random(seed.GetHashCode());
    }
    public RandomConfiguration(string seed = "") {
        if (seed == "") {
            randomDevice = new Random();

            var seedBuilder = new StringBuilder();
            var len = randomDevice.Next(3, 70);
            for (var i = 0; i < len; i++) {
                seedBuilder.Append(chars[randomDevice.Next(chars.Length)]);
            }

            seed = seedBuilder.ToString();
            randomDevice = new Random(seed.GetHashCode());
        } else {
            randomDevice = new Random(seed.GetHashCode());
            this.seed = string.Copy(seed);
        }
    }

    public int Next () {
        return randomDevice.Next();
    }
    public int Next (int maxValue) { 
        return randomDevice.Next(maxValue);
    }
    public int Next (int minValue, int maxValue) {
        return randomDevice.Next(minValue, maxValue);
    }

    public double NextNormal (double mean = 0, double deviation = 1) {
        double u1 = 0;
        while (u1 <= 0) {
            u1 = randomDevice.NextDouble();
        }

        double u2 = randomDevice.NextDouble();

        double r = Math.Sqrt( -2.0*Math.Log(u1) );
        double theta = 2.0 * Math.PI * u2;
        
        return mean + r * Math.Sin(theta) * deviation;
    }

    public bool Binrary (double probability) {
        return randomDevice.NextDouble() < probability;
    }
}