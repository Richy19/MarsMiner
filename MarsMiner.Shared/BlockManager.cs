/**
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarsMiner.Shared
{
    public struct BlockType
    {
        public String Name;

        public bool Solid;

        public Face SolidFaces;
    }

    public static class BlockManager
    {
        private static UInt16 myNextID = 0;
        private static List<BlockType> myBlockTypes;

        public static UInt16 RegisterType( BlockType type )
        {
            if ( myBlockTypes.Count == 0xFFFF )
                throw new Exception( "No more than 65536 block types can be registered." );

            myBlockTypes.Add( type );
            return myNextID++;
        }
    }
}