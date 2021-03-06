﻿/**
 * Copyright (c) 2012 James King [metapyziks@gmail.com]
 *
 * This file is part of MarsMiner.
 * 
 * MarsMiner is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * MarsMiner is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with MarsMiner. If not, see <http://www.gnu.org/licenses/>.
 */

using System;

using MarsMiner.Shared.Octree;

namespace MarsMiner.Shared.Geometry
{
    public class WorldGenerator
    {
        public int Seed { get; private set; }

        public WorldGenerator( int seed = 0 )
        {
            if ( seed == 0 )
            {
                Random rand = new Random();
                seed = rand.Next( int.MaxValue );
            }

            Seed = seed;
        }

        public virtual Octree<UInt16> Generate( int x, int y, int z, int size, int resolution = 1 )
        {
            return new Octree<UInt16>( x, y, z, size );
        }
    }
}
