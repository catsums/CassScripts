using System;
using System.Collections.Generic;
using UnityEngine;

namespace BinaryFlagsNS{
	public class BinFlags{
		List<bool> flags = new List<bool>();
		
		public BinFlags(int val){
			flags = DegToBin(val);
		}

		public static List<bool> DegToBin(int deg){
			List<bool> bin = new List<bool>();

			int temp = deg;

			while(temp > 0){
				bool bit = (temp % 2 == 1) ? true : false;
				
				bin.Add(bit);
				temp /= 2;
			}

			bin.Reverse();

			return bin;
		}

		public static int BinToDeg(List<bool> bits){
			int val = 0;

			for(int i=0;i<bits.Count;i++){
				double index = (bits.Count - i - 1);
				int num = (bits[i]?0:1) * ((int) Math.Pow((double)2, index));

				val += num;
			}

			return val;
		}

		public bool Equals(BinFlags obj){
			if(base.Equals(obj)) return true;

			return (obj.ToInt() == this.ToInt());
		}

		public int ToInt(){
			return BinToDeg(flags);
		}

		public int[] ToIntArray(){
			int[] arr = new int[flags.Count];
			for(int i=0; i < flags.Count; i++){
				arr[i] = (flags[i] ? 1 : 0);
			}
			return arr;
		}
		public bool[] ToBoolArray(){
			bool[] arr = new bool[flags.Count];
			for(int i=0; i < flags.Count; i++){
				arr[i] = (flags[i]);
			}
			return arr;
		}
		
		public override string ToString(){
			string _out = "";
			for(int i=0; i < flags.Count; i++){
				_out += (flags[i] ? "1" : "0");
			}
			return _out;
		}

		public bool GetFlag(int index){
			if(index < 0 || index >= flags.Count) return false;

			return flags[index];
		}
		public bool SetFlag(int index, bool flag){
			if(index < 0 || index >= flags.Count) return false;

			flags[index] = flag;
			return true;
		}

		public bool SetFlags(bool[] arr){
			List<bool> newFlags = new List<bool>();

			foreach (var flag in arr){
				newFlags.Add(flag);
			}

			flags = newFlags;
			return true;
		}

	}
}