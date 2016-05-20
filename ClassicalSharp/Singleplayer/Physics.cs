﻿// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.Collections.Generic;
using ClassicalSharp.Map;
using OpenTK;

namespace ClassicalSharp.Singleplayer {

	public class Physics {
		Game game;
		World map;
		Random rnd = new Random();
		BlockInfo info;
		int width, length, height, oneY;
		
		public const uint tickMask = 0xF8000000;
		public const uint posMask =  0x07FFFFFF;
		public const int tickShift = 27;
		FallingPhysics falling;
		TNTPhysics tnt;
		FoilagePhysics foilage;
		LiquidPhysics liquid;
		
		bool enabled = true;
		public bool Enabled {
			get { return enabled; }
			set { enabled = value; ClearQueuedEvents(); }
		}
		
		public Action<int, byte>[] OnActivate = new Action<int, byte>[256];
		public Action<int, byte>[] OnPlace = new Action<int, byte>[256];
		
		public Physics( Game game ) {
			this.game = game;
			map = game.World;
			info = game.BlockInfo;
			game.WorldEvents.OnNewMapLoaded += ResetMap;
			enabled = Options.GetBool( OptionsKey.SingleplayerPhysics, true );
			
			falling = new FallingPhysics( game, this );
			tnt = new TNTPhysics( game, this );
			foilage = new FoilagePhysics( game, this );
			liquid = new LiquidPhysics( game, this );
		}
		
		internal static bool CheckItem( Queue<uint> queue, out int posIndex ) {
			uint packed = queue.Dequeue();
			int tickDelay = (int)((packed & tickMask) >> tickShift);
			posIndex = (int)(packed & posMask);

			if( tickDelay > 0 ) {
				tickDelay--;
				queue.Enqueue( (uint)posIndex | ((uint)tickDelay << tickShift) );
				return false;
			}
			return true;
		}
		
		internal static bool CheckItem( Queue<uint> queue, int mask, out int posIndex, out int flags ) {
			uint packed = queue.Dequeue();
			flags = (int)((packed & tickMask) >> tickShift);
			posIndex = (int)(packed & posMask);
			int tickDelay = flags & mask;

			if( tickDelay > 0 ) {
				tickDelay--;
				flags &= ~mask; // zero old tick delay bits
				flags |= tickDelay; // then set them with new value
				queue.Enqueue( (uint)posIndex | ((uint)flags << tickShift) );
				return false;
			}
			return true;
		}
		
		int tickCount = 0;
		public void Tick() {
			if( !Enabled || game.World.IsNotLoaded ) return;
			
			//if( (tickCount % 5) == 0 ) {
			liquid.TickLava();
			liquid.TickWater();
			falling.Tick();
			//}
			tickCount++;
			TickRandomBlocks();
		}
		
		public void OnBlockPlaced( int x, int y, int z, byte block ) {
			if( !Enabled ) return;
			int index = (y * length + z) * width + x;
			Action<int, byte> place = OnPlace[block];
			if( place != null ) place( index, block );
		}
		
		void ResetMap( object sender, EventArgs e ) {
			falling.ResetMap();
			liquid.ResetMap();
			width = map.Width;
			height = map.Height;
			length = map.Length;
			oneY = width * length;
		}
		
		void ClearQueuedEvents() {
			liquid.Clear();
			falling.Clear();
		}
		
		public void Dispose() {
			game.WorldEvents.OnNewMapLoaded -= ResetMap;
		}

		
		void TickRandomBlocks() {
			int xMax = width - 1, yMax = height - 1, zMax = length - 1;
			for( int y = 0; y < height; y += 16 )
				for( int z = 0; z < length; z += 16 )
					for( int x = 0; x < width; x += 16 )
			{
				int lo = (y * length + z) * width + x;
				int hi = (Math.Min( yMax, y + 15 ) * length + Math.Min( zMax, z + 15 ))
					* width + Math.Min( xMax, x + 15 );
				
				// Inlined 3 random ticks for this chunk
				int index = rnd.Next( lo, hi );
				byte block = map.blocks[index];
				Action<int, byte> activate = OnActivate[block];
				if( activate != null ) activate( index, block );
				
				index = rnd.Next( lo, hi );
				block = map.blocks[index];
				activate = OnActivate[block];
				if( activate != null ) activate( index, block );
				
				index = rnd.Next( lo, hi );
				block = map.blocks[index];
				activate = OnActivate[block];
				if( activate != null ) activate( index, block );
			}
		}
	}
}