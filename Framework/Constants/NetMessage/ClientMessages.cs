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

namespace Framework.Constants.NetMessage
{
    public enum ClientMessage : ushort
    {
        #region ChatMessages
        ChatMessageSay              = 0x0866,
        ChatMessageYell             = 0x03AB,
        ChatMessageWhisper          = 0x0D8C,
        #endregion

        #region UserRouterClient
        AuthSession                 = 0x196E,
        Ping                        = 0x18E2,
        LogDisconnect               = 0x19EA,
        #endregion

        #region Legacy
        ActivePlayer                = 0x0A12,
        #endregion

        // Value > 0x1FFF are not known.
        #region JAM
        ObjectUpdateFailed          = 0x0A95,
        ViolenceLevel               = 0x0448,
        DBQueryBulk                 = 0x0676,
        GenerateRandomCharacterName = 0x0DD1,
        EnumCharacters              = 0x047C,
        PlayerLogin                 = 0x0754,
        LoadingScreenNotify         = 0x0650,
        SetActionButton             = 0x0D5E,
        CreateCharacter             = 0x077B,
        QueryPlayerName             = 0x05F4,
        QueryRealmName              = 0x0472,
        ReadyForAccountDataTimes    = 0x047F,
        UITimeRequest               = 0x0574,
        CharDelete                  = 0x067A,
        CliSetSpecialization        = 0x085F,
        CliLearnTalents             = 0x026C,
        CliQueryCreature            = 0x0C4A,
        CliQueryGameObject          = 0x08BC,
        CliQueryNPCText             = 0x006C,
        CliTalkToGossip             = 0x02EF,
        CliLogoutRequest            = 0x0183,
        CliSetSelection             = 0x0AC5,
        #endregion

        #region PlayerMove
        MoveStartForward            = 0x112B,
        MoveStartBackward           = 0x02B6,
        MoveStop                    = 0x0609,
        MoveStartStrafeLeft         = 0x0626,
        MoveStartStrafeRight        = 0x048A,
        MoveStopStrafe              = 0x027C,
        MoveJump                    = 0x042F,
        MoveStartTurnLeft           = 0x0409,
        MoveStartTurnRight          = 0x04A0,
        MoveStopTurn                = 0x0488,
        MoveStartPitchUp            = 0x00B6,
        MoveStartPitchDown          = 0x062B,
        MoveStopPitch               = 0x00F5,
        MoveSetRunMode              = 0x0426,
        MoveSetWalkMode             = 0x0625,
        MoveFallLand                = 0x00BC,
        MoveStartSwim               = 0x007E,
        MoveStopSwim                = 0x027F,
        MoveToggleCollisionCheat    = 0x0037,
        MoveSetFacing               = 0x04AF,
        MoveSetPitch                = 0x0689,
        MoveHeartbeat               = 0x04AD,
        MoveFallReset               = 0x007F,
        MoveSetFly                  = 0x060C,
        MoveStartAscend             = 0x0688,
        MoveStopAscend              = 0x06AA,
        MoveChangeTransport         = 0x040F,
        MoveStartDescend            = 0x062A,
        MoveDismissVehicle          = 0x0428,
        #endregion

        TransferInitiate            = 0x4F57,
    }
}
