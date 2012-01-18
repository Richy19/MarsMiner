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

using LibNoise;

using MarsMiner.Shared.Octree;

namespace MarsMiner.Shared.Geometry
{
    public class PerlinGenerator : WorldGenerator
    {
        private Perlin myHillyNoise;
        private Perlin myPlainsNoise;
        private Perlin myTransNoise;

        private int myMinHilly;
        private int myMaxHilly;

        private int myMinPlains;
        private int myMaxPlains;

        public PerlinGenerator( int seed = 0 )
            : base( seed )
        {
            myHillyNoise = new Perlin
            {
                Seed = Seed,
                OctaveCount = 6,
                Frequency = 1.0,
                Lacunarity = 2.0,
                Persistence = 0.5
            };

            myPlainsNoise = new Perlin
            {
                Seed = Seed,
                OctaveCount = 6,
                Frequency = 8.0,
                Lacunarity = 2.0,
                Persistence = 1.0
            };

            myTransNoise = new Perlin
            {
                Seed = Seed,
                OctaveCount = 6,
                Frequency = 1.0 / 32.0,
                Lacunarity = 2.0,
                Persistence = 1.0
            };

            myMinHilly = 64;
            myMaxHilly = 240;

            myMinPlains = 64;
            myMaxPlains = 72;
        }

        public override Octree<UInt16> Generate( int x, int y, int z, int size, int resolution = 1 )
        {
            Octree<UInt16> octree = base.Generate( x, y, z, size, resolution );

            Random rand = new Random( Seed );

            UInt16 empty = BlockManager.FindID( "Core_Empty" );
            UInt16 sand = BlockManager.FindID( "MarsMiner_Sand" );
            UInt16 rock = BlockManager.FindID( "MarsMiner_Rock" );
            UInt16 boulder = BlockManager.FindID( "MarsMiner_Boulder" );

            int min = System.Math.Min( myMinHilly, myMinPlains );
            int gradRange = 2;

            octree.SetCuboid( x, 0, z, size, System.Math.Min( myMinHilly, myMinPlains ), size, rock );

            if ( y + size > min )
            {
                int hillDiff = ( myMaxHilly - myMinHilly ) / 2;
                int hillMid = myMinHilly + hillDiff;
                int plainDiff = ( myMinPlains - myMaxPlains ) / 2;
                int plainMid = myMaxPlains + plainDiff;
                
                int maxCount = size / resolution;
                double hres = resolution / 2.0;

                int[,] heightmap = new int[ maxCount + gradRange * 2, maxCount + gradRange * 2 ];
                double[,] gradmap = new double[ maxCount, maxCount ];

                for( int i = 0; i < maxCount + gradRange * 2; ++ i )
                {
                    double dx = ( x + ( i - gradRange ) * resolution + hres ) / 256.0;
                    for( int j = 0; j < maxCount + gradRange * 2; ++ j )
                    {
                        double dy = ( z + ( j - gradRange ) * resolution + hres ) / 256.0;
                        double hillVal = Tools.Clamp( myHillyNoise.GetValue( dx, dy, 0.5 ) * hillDiff + hillMid, myMinHilly, myMaxHilly );
                        double plainVal = Tools.Clamp( myHillyNoise.GetValue( dx, dy, 0.5 ) * plainDiff + plainMid, myMinPlains, myMaxPlains );
                        double trans = Tools.Clamp( ( myTransNoise.GetValue( dx, dy, 0.5 ) + 1.0 ) / 2.0, 0.0, 1.0 );
                        //trans *= trans;

                        heightmap[ i, j ] = (int) System.Math.Round( ( trans * hillVal + ( 1 - trans ) * plainVal ) / resolution ) * resolution;
                    }
                }

                for ( int i = 0; i < maxCount; ++i )
                {
                    for ( int j = 0; j < maxCount; ++j )
                    {
                        double grad = 0;

                        for ( int gx = -gradRange; gx <= gradRange; ++gx )
                        {
                            for ( int gy = -gradRange; gy <= gradRange; ++gy )
                            {
                                if( gx == 0 && gy == 0 )
                                    continue;

                                double dist = System.Math.Sqrt( gx * gx + gy * gy );

                                int diff = heightmap[ i + gradRange + gx, j + gradRange + gy ]
                                    - heightmap[ i + gradRange, j + gradRange ];

                                grad += System.Math.Abs( diff ) / dist;
                            }
                        }

                        gradmap[ i, j ] = grad;
                    }
                }

                Cuboid rcuboid = new Cuboid( 0, 0, 0, resolution, 1, resolution );
                Cuboid scuboid = new Cuboid( 0, 0, 0, 1, 1, 1 );

                int[,] prev = null;

                for ( int count = 1; count <= maxCount; count <<= 1 )
                {
                    int res = size / count;
                    hres = res / 2.0;
                    int[,] cur = new int[ count, count ];

                    int sca = res / resolution;

                    rcuboid.Width = res;
                    rcuboid.Depth = res;

                    scuboid.Width = res;
                    scuboid.Height = res;
                    scuboid.Depth = res;

                    for ( int nx = 0; nx < count; ++nx )
                    {
                        int rx = x + nx * res;
                        int px = nx >> 1;

                        for ( int nz = 0; nz < count; ++nz )
                        {
                            int rz = z + nz * res;
                            int pz = nz >> 1;

                            int height = heightmap[ nx * sca + gradRange, nz * sca + gradRange ] / res * res;

                            cur[ nx, nz ] = height;

                            int prevHeight = ( count == 1 ? 0 : prev[ px, pz ] );

                            rcuboid.X = rx;
                            rcuboid.Z = rz;

                            rcuboid.Bottom = System.Math.Min( height, prevHeight );
                            rcuboid.Top = System.Math.Max( height, prevHeight );

                            if( height > prevHeight )
                                octree.SetCuboid( rcuboid, rock );
                            else
                                octree.SetCuboid( rcuboid, empty );

                            if ( res == resolution && gradmap[ nx, nz ] <= 8.0 * res )
                            {
                                scuboid.X = rx;
                                scuboid.Y = height - res;
                                scuboid.Z = rz;

                                octree.SetCuboid( scuboid, sand );

                                if ( res == 1 && rand.NextDouble() < 1.0 / 256.0 )
                                {
                                    scuboid.Y += 1;
                                    octree.SetCuboid( scuboid, rock );
                                }
                            }
                        }
                    }

                    prev = cur;
                }
            }

            return octree;
        }
    }
}