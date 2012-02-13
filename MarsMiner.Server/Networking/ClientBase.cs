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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MarsMiner.Shared.Networking;

namespace MarsMiner.Server.Networking
{
    public class ClientBase : RemoteNetworkedObject
    {
        protected static readonly ClientPacketType PTAliveCheck =
            PacketManager.Register( "AliveCheck", delegate( ClientBase sender,
                ClientPacketType type, Stream stream )
        {
            return sender.OnReceiveAliveCheck( stream );
        } );

        protected static readonly ClientPacketType PTPacketDictionary =
            PacketManager.Register( "PacketDictionary", delegate( ClientBase sender,
                ClientPacketType type, Stream stream )
        {
            return sender.OnReceivePacketDictionary( stream );
        } );

        protected override bool ReadPacket( Stream stream )
        {
            base.ReadPacket( stream );
            return PacketManager.HandlePacket( this, stream );
        }

        public override Stream StartPacket( String typeName )
        {
            return StartPacket( PacketManager.GetType( typeName ) );
        }

        public void SendPacketDictionary()
        {
            Stream stream = StartPacket( PTPacketDictionary );
            BinaryWriter writer = new BinaryWriter( stream );
            ClientPacketType[] types = PacketManager.GetAllTypes();
            writer.Write( (UInt16) types.Length );
            foreach ( ClientPacketType t in types )
            {
                writer.Write( t.Name );
                writer.Write( t.ID );
            }
        }

        protected bool OnReceivePacketDictionary( Stream stream )
        {
            return true;
        }
    }
}
