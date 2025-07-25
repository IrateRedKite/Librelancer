﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System.Collections.Generic;
using System.IO;
using LibreLancer.Data.Ini;
using LibreLancer.Data.IO;

namespace LibreLancer.Data.Solar;

public class AsteroidArchIni
{
    public List<Asteroid> Asteroids = new();
    public List<DynamicAsteroid> DynamicAsteroids = new();

    public void AddFile(string path, FileSystem vfs)
    {
        using var stream = vfs == null ? File.OpenRead(path) : vfs.Open(path);
        foreach (var s in IniFile.ParseFile(path, stream))
            switch (s.Name.ToLowerInvariant())
            {
                case "asteroid":
                    if (Asteroid.TryParse(s, out var asteroid))
                    {
                        Asteroids.Add(asteroid);
                    }
                    break;
                case "asteroidmine":
                    if (Asteroid.TryParse(s, out var asteroidMine))
                    {
                        asteroidMine.IsMine = true;
                        Asteroids.Add(asteroidMine);
                    }
                    break;
                case "dynamicasteroid":
                    if (DynamicAsteroid.TryParse(s, out var dynamicAsteroid))
                    {
                        DynamicAsteroids.Add(dynamicAsteroid);
                    }
                    break;
            }
    }
}
