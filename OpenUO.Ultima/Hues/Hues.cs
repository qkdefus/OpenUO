﻿#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   Hues.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.IO;

#endregion

namespace OpenUO.Ultima
{
    public class Hues
    {
        private readonly Hue[] _hues;

        public Hues(InstallLocation install)
        {
            var path = install.GetPath("hues.mul");
            var index = 0;

            _hues = new Hue[3000];

            if(path != null)
            {
                using(var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bin = new BinaryReader(fs);

                    var blockCount = (int)fs.Length / 708;

                    if(blockCount > 375)
                    {
                        blockCount = 375;
                    }

                    for(var i = 0; i < blockCount; ++i)
                    {
                        bin.ReadInt32();

                        for(var j = 0; j < 8; ++j, ++index)
                        {
                            _hues[index] = new Hue(index, bin);
                        }
                    }
                }
            }

            for(; index < 3000; ++index)
            {
                _hues[index] = new Hue(index);
            }
        }

        public Hue[] Table
        {
            get { return _hues; }
        }

        public Hue GetHue(int index)
        {
            index &= 0x3FFF;

            if(index >= 0 && index < 3000)
            {
                return _hues[index];
            }

            return _hues[0];
        }
    }
}