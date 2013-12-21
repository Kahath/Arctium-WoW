/*
 * Copyright (C) 2012-2013 Arctium <http://arctium.org>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using Framework.Constants.Movement;
using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class MoveHandler : Globals
    {
        [Opcode(ClientMessage.MoveStartForward, "17658")]
        public static void HandleMoveStartForward(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            guidMask[3] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var HasTime = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }
            }

            if (Unknown2)
                packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartBackward, "17658")]
        public static void HandleMoveStartBackward(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            var Unknown3 = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[7] = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStop, "17658")]
        public static void HandleMoveStop(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            var Unknown4 = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var HasPitch = !BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartStrafeLeft, "17658")]
        public static void HandleMoveStartStrafeLeft(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            movementValues.IsTransport = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (HasPitch)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartStrafeRight, "17658")]
        public static void HandleMoveStartStrafeRight(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            guidMask[2] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (HasPitch)
                packet.Read<float>();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStopStrafe, "17658")]
        public static void HandleMoveStopStrafe(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            var Unknown3 = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var HasTime = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (HasPitch)
                packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveJump, "17658")]
        public static void HandleMoveJump(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            guidMask[4] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[6] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartTurnLeft, "17658")]
        public static void HandleMoveStartTurnLeft(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            var HasTime = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[4] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.FallTime = packet.Read<uint>();

                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (Unknown2)
                packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartTurnRight, "17658")]
        public static void HandleMoveStartTurnRight(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            var counter = BitUnpack.GetBits<uint>(22);

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();

                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                }
            }

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStopTurn, "17658")]
        public static void HandleMoveStopTurn(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            guidMask[2] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }
            }

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartPitchUp, "17658")]
        public static void HandleMoveStartPitchUp(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var HasSplineElevation = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (HasPitch)
                packet.Read<float>();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.FallTime = packet.Read<uint>();

                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartPitchDown, "17658")]
        public static void HandleMoveStartPitchDown(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            guidMask[4] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[6] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStopPitch, "17658")]
        public static void HandleMoveStopPitch(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveSetRunMode, "17658")]
        public static void HandleMoveSetRunMode(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            guidMask[1] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var HasSplineElevation = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.Cos = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                }
            }

            if (HasPitch)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveSetWalkMode, "17658")]
        public static void HandleMoveSetWalkMode(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            guidMask[4] = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveFallLand, "17658")]
        public static void HandleMoveFallLand(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            var HasTime = !BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[6] = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (HasPitch)
                packet.Read<float>();

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartSwim, "17658")]
        public static void HandleMoveStartSwim(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            guidMask[0] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStopSwim, "17658")]
        public static void HandleMoveStopSwim(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            guidMask[4] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var Unknown3 = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (HasPitch)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveToggleCollisionCheat, "17658")]
        public static void HandleMoveToggleCollisionCheat(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var HasSplineElevation = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasPitch)
                packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveSetFacing, "17658")]
        public static void HandleMoveSetFacing(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
            }

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveSetPitch, "17658")]
        public static void HandleMoveSetPitch(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            guidMask[3] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (HasSplineElevation)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (HasPitch)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveHeartbeat, "17658")]
        public static void HandleMoveHeartbeat(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            guidMask[2] = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var Unknown3 = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveFallReset, "17658")]
        public static void HandleMoveFallReset(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            guidMask[1] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var Unknown2 = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
            }

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveSetFly, "17658")]
        public static void HandleMoveSetFly(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
            };

            var Unknown3 = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            movementValues.IsTransport = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasPitch)
                packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartAscend, "17658")]
        public static void HandleMoveStartAscend(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            guidMask[3] = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            var Unknown2 = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStopAscend, "17658")]
        public static void HandleMoveStopAscend(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            var HasSplineElevation = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[1] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();

                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }
            }

            if (HasPitch)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveChangeTransport, "17658")]
        public static void HandleMoveChangeTransport(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
            };

            guidMask[4] = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[6] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
                movementValues.FallTime = packet.Read<uint>();
            }

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasPitch)
                packet.Read<float>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveStartDescend, "17658")]
        public static void HandleMoveStartDescend(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Z = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            var Unknown2 = !BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[5] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.FallTime = packet.Read<uint>();

                if (movementValues.HasJumpData)
                {
                    movementValues.Cos = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }

                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (HasSplineElevation)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        [Opcode(ClientMessage.MoveDismissVehicle, "17658")]
        public static void HandleMoveDismissVehicle(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Z = packet.Read<float>(),
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
            };

            guidMask[6] = BitUnpack.GetBit();

            var Unknown4 = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            var Unknown2 = !BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown3 = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();

            guidMask[5] = BitUnpack.GetBit();

            guidMask[2] = BitUnpack.GetBit();

            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            guidMask[3] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            guidMask[1] = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[1])
                guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[3])
                guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[7])
                guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[6])
                guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[0])
                guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[4])
                guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[5])
                guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);

            if (guidMask[2])
                guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (Unknown2)
                packet.Read<uint>();

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();
            }

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (HasPitch)
                packet.Read<float>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);
        }

        public static void HandleMoveUpdate(ulong guid, ObjectMovementValues movementValues, Vector4 vector)
        {
            PacketWriter moveUpdate = new PacketWriter(ServerMessage.MoveUpdate);
            BitPack BitPack = new BitPack(moveUpdate, guid);


            BitPack.WriteGuidMask(7);
            BitPack.Write(!movementValues.HasMovementFlags2);
            BitPack.Write(1);
            BitPack.Write(1);
            BitPack.Write(0);
            BitPack.Write(movementValues.Time == 0);
            BitPack.WriteGuidMask(6);
            BitPack.Write(0);
            BitPack.WriteGuidMask(0);
            BitPack.Write(movementValues.IsTransport);
            BitPack.WriteGuidMask(4);
            BitPack.Write(!movementValues.HasMovementFlags);
            BitPack.WriteGuidMask(5, 3);
            BitPack.Write(!movementValues.HasRotation);
            BitPack.WriteGuidMask(1);
            BitPack.Write(1);
            BitPack.WriteGuidMask(2);
            BitPack.Write<uint>(0, 22);

            if (movementValues.HasMovementFlags2)
                BitPack.Write((uint)movementValues.MovementFlags2, 13);

            BitPack.Write(0);

            if (movementValues.HasMovementFlags)
                BitPack.Write((uint)movementValues.MovementFlags, 30);

            BitPack.Write(movementValues.IsFallingOrJumping);

            if (movementValues.IsFallingOrJumping)
                BitPack.Write(movementValues.HasJumpData);

            BitPack.Flush();

            if (movementValues.IsFallingOrJumping)
            {
                if (movementValues.HasJumpData)
                {
                    moveUpdate.WriteFloat(movementValues.CurrentSpeed);
                    moveUpdate.WriteFloat(movementValues.Sin);
                    moveUpdate.WriteFloat(movementValues.Cos);
                }

                moveUpdate.WriteUInt32(movementValues.FallTime);
                moveUpdate.WriteFloat(movementValues.JumpVelocity);
            }

            BitPack.WriteGuidBytes(3);

            moveUpdate.WriteFloat(vector.Y);

            BitPack.WriteGuidBytes(5, 0, 7, 6);

            moveUpdate.WriteFloat(vector.Z);

            BitPack.WriteGuidBytes(1);

            if (movementValues.Time != 0)
                moveUpdate.WriteUInt32(movementValues.Time);

            moveUpdate.WriteFloat(vector.X);

            if (movementValues.HasRotation)
                moveUpdate.WriteFloat(vector.O);

            BitPack.WriteGuidBytes(2, 4);

            var session = WorldMgr.GetSession(guid);
            if (session != null)
            {
                Character pChar = session.Character;

                ObjectMgr.SetPosition(ref pChar, vector, false);
                WorldMgr.SendToInRangeCharacter(pChar, moveUpdate);
            }
        }

        public static void HandleMoveSetWalkSpeed(ref WorldClass session, float speed = 2.5f)
        {
            PacketWriter setWalkSpeed = new PacketWriter(ServerMessage.MoveSetWalkSpeed);
            BitPack BitPack = new BitPack(setWalkSpeed, session.Character.Guid);

            BitPack.WriteGuidMask(2, 6, 1, 0, 3, 5, 4, 7);

            BitPack.Flush();

            BitPack.WriteGuidBytes(7, 5);

            setWalkSpeed.WriteFloat(speed);

            BitPack.WriteGuidBytes(2, 3, 1, 0, 6);

            setWalkSpeed.WriteUInt32(0);

            session.Send(ref setWalkSpeed);
        }

        public static void HandleMoveSetRunSpeed(ref WorldClass session, float speed = 7f)
        {
            PacketWriter setRunSpeed = new PacketWriter(ServerMessage.MoveSetRunSpeed);
            BitPack BitPack = new BitPack(setRunSpeed, session.Character.Guid);

            BitPack.WriteGuidMask(3, 4, 5, 7, 1, 2, 6, 0);

            BitPack.Flush();

            BitPack.WriteGuidBytes(5, 3);

            setRunSpeed.WriteFloat(speed);

            BitPack.WriteGuidBytes(2, 7);

            setRunSpeed.WriteUInt32(0);

            BitPack.WriteGuidBytes(1, 6, 0, 4);

            session.Send(ref setRunSpeed);
        }

        public static void HandleMoveSetSwimSpeed(ref WorldClass session, float speed = 4.72222f)
        {
            PacketWriter setSwimSpeed = new PacketWriter(ServerMessage.MoveSetSwimSpeed);
            BitPack BitPack = new BitPack(setSwimSpeed, session.Character.Guid);

            BitPack.WriteGuidMask(5, 1, 4, 3, 0, 7, 2, 6);

            BitPack.Flush();

            BitPack.WriteGuidBytes(7);

            setSwimSpeed.WriteFloat(speed);

            BitPack.WriteGuidBytes(5, 2, 6, 1, 4);

            setSwimSpeed.WriteUInt32(0);

            BitPack.WriteGuidBytes(3, 0);

            session.Send(ref setSwimSpeed);
        }

        public static void HandleMoveSetFlightSpeed(ref WorldClass session, float speed = 7f)
        {
            PacketWriter setFlightSpeed = new PacketWriter(ServerMessage.MoveSetFlightSpeed);
            BitPack BitPack = new BitPack(setFlightSpeed, session.Character.Guid);

            BitPack.WriteGuidMask(1, 5, 3, 0, 6, 7, 4, 2);

            BitPack.Flush();

            BitPack.WriteGuidBytes(4, 5, 1, 3, 2);

            setFlightSpeed.WriteUInt32(0);

            BitPack.WriteGuidBytes(6, 0);

            setFlightSpeed.WriteFloat(speed);

            BitPack.WriteGuidBytes(7);

            session.Send(ref setFlightSpeed);
        }

        public static void HandleMoveSetCanFly(ref WorldClass session)
        {
            PacketWriter moveSetCanFly = new PacketWriter(ServerMessage.MoveSetCanFly);
            BitPack BitPack = new BitPack(moveSetCanFly, session.Character.Guid);

            BitPack.WriteGuidMask(2, 4, 7, 0, 6, 3, 5, 1);

            BitPack.Flush();

            BitPack.WriteGuidBytes(1, 6, 0, 3, 5, 7);

            moveSetCanFly.WriteUInt32(0);

            BitPack.WriteGuidBytes(4);

            session.Send(ref moveSetCanFly);
        }

        public static void HandleMoveUnsetCanFly(ref WorldClass session)
        {
            PacketWriter unsetCanFly = new PacketWriter(ServerMessage.MoveUnsetCanFly);
            BitPack BitPack = new BitPack(unsetCanFly, session.Character.Guid);

            BitPack.WriteGuidMask(2, 6, 1, 7, 4, 5, 3, 0);

            BitPack.Flush();

            BitPack.WriteGuidBytes(2, 4, 5, 1, 7, 6, 3, 0);

            unsetCanFly.WriteUInt32(0);

            session.Send(ref unsetCanFly);
        }

        public static void HandleMoveTeleport(ref WorldClass session, Vector4 vector)
        {
            bool isTransport = false;
            bool unknown = false;

            PacketWriter moveTeleport = new PacketWriter(ServerMessage.MoveTeleport);
            BitPack BitPack = new BitPack(moveTeleport, session.Character.Guid);

            BitPack.Write(isTransport);
            BitPack.WriteGuidMask(3);

            if (isTransport)
                BitPack.WriteTransportGuidMask(1, 3, 6, 4, 5, 7, 0, 2);

            BitPack.WriteGuidMask(4, 5, 0);

            BitPack.Write(unknown);

            if (unknown)
            {
                BitPack.Write(0);
                BitPack.Write(0);
            }

            BitPack.WriteGuidMask(7, 6, 2, 1);

            BitPack.Flush();

            if (isTransport)
                BitPack.WriteTransportGuidBytes(2, 3, 5, 0, 4, 6, 1, 7);

            if (unknown)
                moveTeleport.WriteUInt8(0);

            BitPack.WriteGuidBytes(6, 1);

            moveTeleport.WriteFloat(vector.X);
            moveTeleport.WriteFloat(vector.O);

            BitPack.WriteGuidBytes(3);

            moveTeleport.WriteFloat(vector.Z);
            moveTeleport.WriteUInt32(0);
            moveTeleport.WriteFloat(vector.Y);

            BitPack.WriteGuidBytes(7, 4, 5, 0, 2);

            session.Send(ref moveTeleport);
        }

        public static void HandleTransferPending(ref WorldClass session, uint mapId)
        {
            var unknown = false;
            var isTransport = false;

            PacketWriter transferPending = new PacketWriter(ServerMessage.TransferPending);
            BitPack BitPack = new BitPack(transferPending);

            BitPack.Write(unknown);
            BitPack.Write(isTransport);

            BitPack.Flush();

            transferPending.WriteUInt32(mapId);

            if (isTransport)
            {
                transferPending.WriteUInt32(0);
                transferPending.WriteUInt32(0);
            }

            if (unknown)
                transferPending.WriteUInt32(0);

            session.Send(ref transferPending);
        }

        public static void HandleNewWorld(ref WorldClass session, Vector4 vector, uint mapId)
        {
            PacketWriter newWorld = new PacketWriter(ServerMessage.NewWorld);

            newWorld.WriteFloat(vector.Z);
            newWorld.WriteUInt32(mapId);
            newWorld.WriteFloat(vector.Y);
            newWorld.WriteFloat(vector.O);
            newWorld.WriteFloat(vector.X);

            session.Send(ref newWorld);
        }
    }
}
