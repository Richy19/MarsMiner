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
using System.IO;

namespace MarsMiner.Shared
{
    public class PlayerInfo
    {
        public static PlayerInfo FromStream( Stream stream )
        {
            PlayerInfo info = new PlayerInfo();

            BinaryReader reader = new BinaryReader( stream );
            info.ID = reader.ReadUInt16();
            info.Name = reader.ReadString();

            return info;
        }

        public UInt16 ID { get; private set; }
        public String Name { get; private set; }

        public void Write( Stream stream )
        {
            BinaryWriter writer = new BinaryWriter( stream );
            writer.Write( ID );
            writer.Write( Name );
        }
    }
}
