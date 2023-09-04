using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace MyHelperFunctions
{
	public static class MY{

		static public long SafeDivide(long a, long b){
			long INF = long.MaxValue;
			long NEG_INF = long.MinValue;

			if(a == 0 && b == 0) return 0;
			if(a == 0 && (b == INF || b == NEG_INF)) return 0;
			if((a == INF || a == NEG_INF) && b == 0) return a;
			if((a == INF || a == NEG_INF) && (b == INF || b == NEG_INF)){
				return (a == b ? 1 : -1);
			}
			if(b == 0){
				return (a > 0 ? INF : NEG_INF);
			}
			if(b == INF || b == -INF) return 0;
			
			return (a / b);
		}static public double SafeDivide(double a, double b){
			double INF = double.PositiveInfinity;
			double NEG_INF = double.NegativeInfinity;

			if(a == 0 && b == 0) return 0;
			if(a == 0 && (b == INF || b == NEG_INF)) return 0;
			if((a == INF || a == NEG_INF) && b == 0) return a;
			if((a == INF || a == NEG_INF) && (b == INF || b == NEG_INF)){
				return (a == b ? 1 : -1);
			}
			if(b == 0){
				return (a > 0 ? INF : NEG_INF);
			}
			if(b == INF || b == -INF) return 0;
			
			return (a / b);
		}static public float SafeDivide(float a, float b){
			float INF = float.PositiveInfinity;
			float NEG_INF = float.NegativeInfinity;

			if(a == 0 && b == 0) return 0;
			if(a == 0 && (b == INF || b == NEG_INF)) return 0;
			if((a == INF || a == NEG_INF) && b == 0) return a;
			if((a == INF || a == NEG_INF) && (b == INF || b == NEG_INF)){
				return (a == b ? 1 : -1);
			}
			if(b == 0){
				return (a > 0 ? INF : NEG_INF);
			}
			if(b == INF || b == -INF) return 0;
			
			return (a / b);
		}
		static public bool IsInRange<T>(T num, T min, T max, bool inclusive = true) where T : IComparable
		{
			if(inclusive){
				return (num.CompareTo(min) >= 0 && num.CompareTo(max) <= 0);
			}else{
				return (num.CompareTo(min) > 0 && num.CompareTo(max) < 0);
			}
		}

		static public int MOD(int a, int b){
			return ((a % b) + b) % b;
		}
		static public long MOD(long a, long b){
			return ((a % b) + b) % b;
		}static public decimal MOD(decimal a, decimal b){
			return ((a % b) + b) % b;
		}static public float MOD(float a, float b){
			return ((a % b) + b) % b;
		}static public double MOD(double a, double b){
			return ((a % b) + b) % b;
		}

		static public long SumOfArray(long[] arr, Func<long,long> additionFunc = null){
			if(additionFunc == null){
				additionFunc = (x) => {
					return x;
				};
			}
			long sum = 0;
			foreach(long item in arr){
				sum += additionFunc(item);
			}
			return sum;
		}static public decimal SumOfArray(decimal[] arr, Func<decimal,decimal> additionFunc = null){
			if(additionFunc == null){
				additionFunc = (x) => {
					return x;
				};
			}
			decimal sum = 0;
			foreach(decimal item in arr){
				sum += additionFunc(item);
			}
			return sum;
		}

		static public List<Vector2> GetClosestPathInCircle(List<Vector2> arr, int _from, int _to, bool clockwiseBias = true){
			return MY.GetClosestPathInCircle(arr.ToArray(), _from, _to, clockwiseBias);
		}
		static public List<Vector2> GetClosestPathInCircle(Vector2[] arr, int _from, int _to, bool clockwiseBias = true){
			if(!MY.IsInRange<int>(_from, 0, arr.Length-1) || !MY.IsInRange<int>(_to, 0, arr.Length-1)){
				return new List<Vector2>();
			}

			int iL, iR; iL = iR = _from;

			List<Vector2> arrL = new List<Vector2>();
			List<Vector2> arrR = new List<Vector2>();
			List<Vector2> arrX;

			while(arrL.Count < arr.Length){
				int _i = (int) MY.MOD(iL, arr.Length);

				arrL.Add(arr[_i]);
				if(Math.Abs(_i) == _to) break;
				iL --;
			}
			while(arrR.Count < arr.Length){
				int _i = (int) MY.MOD(iR, arr.Length);

				arrR.Add(arr[_i]);
				if(Math.Abs(_i) == _to) break;
				iR --;
			}

			if(Math.Abs(arrL.Count) < Math.Abs(arrR.Count)){
				arrX = arrL;
			}else if(Math.Abs(arrL.Count) > Math.Abs(arrR.Count)){
				arrX = arrR;
			}else{
				if(clockwiseBias) arrX = arrR;
				else arrX = arrL;
			}

			return arrX;
			
		}

		public static long Lerp(long a, long b, float t) {
			return (long) (a + t * ( b - a ));
		}public static float LerpT(long a, long b, long L) {
			if (b==a) return long.MaxValue;
			return (L-a)/(b-a);
		}
		public static int Lerp(int a, int b, float t) {
			return (int) (a + t * ( b - a ));
		}public static float LerpT(int a, int b, int L) {
			if (b==a) return int.MaxValue;
			return (L-a)/(b-a);
		}
		public static double Lerp(double a, double b, double t) {
			return a + t * ( b - a );
		}public static double LerpT(double a, double b, double L) {
			if (b==a) return double.PositiveInfinity;
			return (L-a)/(b-a);
		}
		public static float Lerp(float a, float b, float t) {
			return a + t * ( b - a );
		}public static float LerpT(float a, float b, float L) {
			if (b==a) return float.PositiveInfinity;
			return (L-a)/(b-a);
		}

		public static float QuadraticBezier(float[] pts, float t){
            var qPoints = new List<float>();

            for(int i=0;i<pts.Length-1;i++){
                var pA = pts[i]; var pB = pts[i+1];

                var pt = Lerp(pA, pB, t);

                qPoints.Add(pt);
            }

            if(qPoints.Count<2) return qPoints[0];

            return QuadraticBezier(qPoints.ToArray(),t);
        }

        public static float QuadraticBezier(float ptA, float ptB, float ptC, float t){
            return QuadraticBezier(new float[]{
                ptA, ptB, ptC
            }, t);
        }
        public static float CubicBezier(float ptA, float ptB, float ptC, float ptD, float t){
            return QuadraticBezier(new float[]{
                ptA, ptB, ptC, ptD
            }, t);
        }

        public static float[] QuadraticBezierPoints(float[] pts, float inc){
            var qPoints = new List<float>();
            if(inc>0){
                float t = 0; bool last = false;
                while(t <= 1){
                    qPoints.Add( QuadraticBezier(pts,t) );
                    t += inc;
                    if(t>1 && !last){
                        t = 1; last = true;
                    }
                }
            }

            return qPoints.ToArray();
        }

		public static T RandomArrayItem<T>(T[] arr){
			int index = RandomInt(arr.Length);
			return arr[index];
		}

		public static int RandomInt(int max, bool inclusive=false){
   			Random random = new Random();
			return random.Next(0, max + (inclusive ? 1:0) );
		}
		public static int RandomInt(int min, int max, bool inclusive=false){
   			Random random = new Random();
			return random.Next(min, max + (inclusive ? 1:0) );
		}
		public static float RandomFloat(float min, float max){
   			Random random = new Random();
			var t = (float) random.NextDouble();
			return Lerp(min,max,t);
		}
		public static double RandomDouble(double min, float max){
   			Random random = new Random();
			var t = random.NextDouble();
			return Lerp(min,max,t);
		}

		public static string RandomString(int len, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"){
   			Random random = new Random();
			return new string(Enumerable.Repeat(chars, len).Select(s => s[random.Next(s.Length)]).ToArray());
		}
		public static string RandomID(int len, string prefix = "", string suffix = ""){
			return $"{prefix}{RandomString(len)}{suffix}";
		}


	}
}