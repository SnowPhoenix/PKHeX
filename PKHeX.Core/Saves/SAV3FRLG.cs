﻿using System;

namespace PKHeX.Core
{
    public sealed class SAV3FRLG : SAV3, IGen3Joyful
    {
        // Configuration
        protected override SaveFile CloneInternal() => new SAV3FRLG(Write());
        public override GameVersion Version { get; protected set; } = GameVersion.FR; // allow mutation
        private PersonalTable _personal = PersonalTable.FR;
        public override PersonalTable Personal => _personal;

        protected override int EventFlagMax => 8 * 288;
        protected override int EventConstMax => 0x100;
        protected override int DaycareSlotSize => SIZE_STORED + 0x3C; // 0x38 mail + 4 exp
        public override int DaycareSeedSize => 4; // 16bit
        protected override int EggEventFlag => 0x266;
        protected override int BadgeFlagStart => 0x820;

        public SAV3FRLG(byte[] data) : base(data) => Initialize();
        public SAV3FRLG(bool japanese = false) : base(japanese) => Initialize();

        private void Initialize()
        {
            // small
            PokeDex = 0x18;

            // large
            EventFlag = 0xEE0;
            EventConst = 0x1000;
            DaycareOffset = 0x2F80;

            // storage
            Box = 0;
        }

        public bool ResetPersonal(GameVersion g)
        {
            if (g is not (GameVersion.FR or GameVersion.LG))
                return false;
            _personal = g == GameVersion.FR ? PersonalTable.FR : PersonalTable.LG;
            return true;
        }

        #region Small
        public override bool NationalDex
        {
            get => PokedexNationalMagicFRLG == PokedexNationalUnlockFRLG;
            set
            {
                PokedexNationalMagicFRLG = value ? PokedexNationalUnlockFRLG : 0; // magic
                SetEventFlag(0x840, value);
                SetEventConst(0x4E, PokedexNationalUnlockWorkFRLG);
            }
        }

        public ushort JoyfulJumpInRow           { get => BitConverter.ToUInt16(Small, 0xB00); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0xB00); }
        // u16 field2;
        public ushort JoyfulJump5InRow          { get => BitConverter.ToUInt16(Small, 0xB04); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0xB04); }
        public ushort JoyfulJumpGamesMaxPlayers { get => BitConverter.ToUInt16(Small, 0xB06); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0xB06); }
        // u32 field8;
        public uint   JoyfulJumpScore           { get => BitConverter.ToUInt16(Small, 0xB0C); set => SetData(Small, BitConverter.GetBytes(Math.Min(        9999, value)), 0xB0C); }

        public uint   JoyfulBerriesScore        { get => BitConverter.ToUInt16(Small, 0xB10); set => SetData(Small, BitConverter.GetBytes(Math.Min(        9999, value)), 0xB10); }
        public ushort JoyfulBerriesInRow        { get => BitConverter.ToUInt16(Small, 0xB14); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0xB14); }
        public ushort JoyfulBerries5InRow       { get => BitConverter.ToUInt16(Small, 0xB16); set => SetData(Small, BitConverter.GetBytes(Math.Min((ushort)9999, value)), 0xB16); }

        public uint BerryPowder
        {
            get => BitConverter.ToUInt32(Small, 0xAF8) ^ SecurityKey;
            set => SetData(Small, BitConverter.GetBytes(value ^ SecurityKey), 0xAF8);
        }

        public override uint SecurityKey
        {
            get => BitConverter.ToUInt32(Small, 0xF20);
            set => SetData(Small, BitConverter.GetBytes(value), 0xF20);
        }
        #endregion

        #region Large
        public override int PartyCount { get => Large[0x034]; protected set => Large[0x034] = (byte)value; }
        public override int GetPartyOffset(int slot) => 0x038 + (SIZE_PARTY * slot);

        public override uint Money
        {
            get => BitConverter.ToUInt32(Large, 0x0290) ^ SecurityKey;
            set => SetData(BitConverter.GetBytes(value ^ SecurityKey), 0x0290);
        }

        public override uint Coin
        {
            get => BitConverter.ToUInt16(Large, 0x0294) ^ SecurityKey;
            set => SetData(BitConverter.GetBytes(value ^ SecurityKey), 0x0294);
        }

        private const int OFS_PCItem = 0x0298;
        private const int OFS_PouchHeldItem = 0x0310;
        private const int OFS_PouchKeyItem = 0x03B8;
        private const int OFS_PouchBalls = 0x0430;
        private const int OFS_PouchTMHM = 0x0464;
        private const int OFS_PouchBerry = 0x054C;

        protected override InventoryPouch3[] GetItems()
        {
            const int max = 999;
            var PCItems = ArrayUtil.ConcatAll(Legal.Pouch_Items_RS, Legal.Pouch_Key_FRLG, Legal.Pouch_Ball_RS, Legal.Pouch_HM_RS, Legal.Pouch_Berries_RS);
            return new InventoryPouch3[]
            {
                new(InventoryType.Items, Legal.Pouch_Items_RS, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
                new(InventoryType.KeyItems, Legal.Pouch_Key_FRLG, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
                new(InventoryType.Balls, Legal.Pouch_Ball_RS, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
                new(InventoryType.TMHMs, Legal.Pouch_HM_RS, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
                new(InventoryType.Berries, Legal.Pouch_Berries_RS, 999, OFS_PouchBerry, 43),
                new(InventoryType.PCItems, PCItems, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
            };
        }

        protected override int SeenOffset2 => 0x5F8;
        protected override int MailOffset => 0x2CD0;

        protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(0, slot + 1) - 4; // @ end of each pkm slot
        public override string GetDaycareRNGSeed(int loc) => BitConverter.ToUInt16(Large, GetDaycareEXPOffset(2)).ToString("X4"); // after the 2nd slot EXP, before the step counter
        public override void SetDaycareRNGSeed(int loc, string seed) => BitConverter.GetBytes((ushort)Util.GetHexValue(seed)).CopyTo(Large, GetDaycareEXPOffset(2));

        #region eBerry
        private const int OFFSET_EBERRY = 0x30EC;
        private const int SIZE_EBERRY = 0x134;

        public byte[] GetEReaderBerry() => Large.Slice(OFFSET_EBERRY, SIZE_EBERRY);
        public void SetEReaderBerry(byte[] data) => SetData(Large, data, OFFSET_EBERRY);

        public override string EBerryName => GetString(Large, OFFSET_EBERRY, 7);
        public override bool IsEBerryEngima => Large[OFFSET_EBERRY] is 0 or 0xFF;
        #endregion

        protected override int SeenOffset3 => 0x3A18;

        public string RivalName
        {
            get => GetString(Large, 0x3A4C, 8);
            set => SetData(SetString(value, 7), 0x3A4C);
        }

        #endregion
    }
}
