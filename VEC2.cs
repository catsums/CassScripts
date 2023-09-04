using System.Numerics;
using System;
using System.Collections.Generic;
using MyHelperFunctions;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace VEC2{
	/*
		C# Extension for Unity Vector2 Functions. Note that it will not work with Vector2Int
	*/
    public static class Vector2Ext{
		/*
			Converts Degrees To Radians
		*/
        public static float DegToRad(float deg){
            return (deg * Mathf.PI)/180;
        }
		/*
			Converts Radiants To Degrees
		*/
        public static float RadToDeg(float rad){
            return (rad * 180)/Mathf.PI;
        }
		public static double DegToRad(double deg){
            return (deg * Mathf.PI)/180;
        }
        public static double RadToDeg(double rad){
            return (rad * 180)/Mathf.PI;
        }


        public static Vector2 Clone(this Vector2 v){
            return new Vector2(v.x,v.y);
        }
        public static float SumOfParts(this Vector2 v){
            return (v.x + v.y);
        }
        public static Vector2 Abs(this Vector2 v){
           return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }
        public static float LengthSquared(this Vector2 v){
            return Mathf.Pow(v.x,2) + Mathf.Pow(v.y, 2);
        }
        public static float Length(this Vector2 v){
            return Mathf.Sqrt(v.LengthSquared());
        }
        public static Vector2 Lerp(this Vector2 v, Vector2 other, float t){
            return new Vector2(
                v.x + (other.x - v.x) * t,
                v.y + (other.y - v.y) * t
            );
        }
        public static Vector2 Ratioed(this Vector2 v){
            var sum = v.SumOfParts();
            return new Vector2(
                MY.SafeDivide(v.x,sum), MY.SafeDivide(v.y,sum)
            );
        }
        public static float Dot(this Vector2 v, Vector2 other){
            return (v.x * other.x)+(v.y * other.y);
        }
        public static float AngleInRadians(this Vector2 v){
			return Mathf.Atan2(v.y, v.x);
        }
        public static float Angle(this Vector2 v){
            return RadToDeg(v.AngleInRadians());
        }
        public static float AngleTo(this Vector2 v, Vector2 other){
            return (v.Angle() - other.Angle());
        }
        public static Vector2 LineTo(this Vector2 v, Vector2 other){
            return (other - v);
        }
        public static float AngleToPoint(this Vector2 v, Vector2 other){
            return v.LineTo(other).Angle();
        }
        public static float DistanceTo(this Vector2 v, Vector2 other){
            return (other - v).Length();
        }
        public static Vector2 Rotated(this Vector2 v, Vector2 pivot, float angleInDeg){
            var angle = DegToRad(angleInDeg);

            var pt = v.Clone();
            var ct = pivot.Clone();

            var sinO = Mathf.Sin(angle);
            var cosO = Mathf.Cos(angle);

            pt.x -= ct.x; pt.y -= ct.y;

            return new Vector2(
                (pt.x * cosO) - (pt.y * sinO) + ct.x,
                (pt.x * sinO) + (pt.y * cosO) + ct.y
            );
        }
        public static void RotateAround(this Vector2 v, Vector2 pivot, float angleInDeg){
            var rotated = v.Rotated(pivot, angleInDeg);
            v.x = rotated.x; v.y = rotated.y;
        }
		public static Vector2 Skew(this Vector2 v, Vector2 pivot, Vector2 skewer){
			var pt = v;
			var ct = pivot;

			pt.x -= ct.x;
			pt.y -= ct.y;

			var _x = (pt.x) + (pt.y * skewer.x) + ct.x;
			var _y = (pt.x * skewer.y) + (pt.y) + ct.y;

			return new Vector2(_x,_y);
		}
		public static void SkewAround(this Vector2 v, Vector2 pivot, Vector2 skewer){
			var pt = v;
			var ct = pivot;

			pt.x -= ct.x;
			pt.y -= ct.y;

			v.x = (pt.x) + (pt.y * skewer.x) + ct.x;
			v.y = (pt.x * skewer.y) + (pt.y) + ct.y;
		}
        public static Vector2 Inverse(this Vector2 v){
            return new Vector2(
                MY.SafeDivide(1,v.x), MY.SafeDivide(1,v.y)
            );
        }
        public static Vector2 Flipped(this Vector2 v){
            return new Vector2(v.y, v.x);
        }
        public static Vector2 MidPoint(Vector2[] arr){
            float x=0, y=0;

            foreach(Vector2 v in arr){
                x += v.x; y += v.y;
            }

            return new Vector2(
                MY.SafeDivide(x, arr.Length), MY.SafeDivide(y,arr.Length)
            );
        }
        public static Vector2 MidPoint(this Vector2 v, Vector2 other){
            float x=0, y=0;
            Vector2[] arr = new Vector2[]{v,other};

            foreach(Vector2 vec in arr){
                x += vec.x; y += vec.y;
            }

            return new Vector2(
                MY.SafeDivide(x, arr.Length), MY.SafeDivide(y,arr.Length)
            );
        }
        public static Vector2 Floor(this Vector2 v){
            return new Vector2(Mathf.Floor(v.x),Mathf.Floor(v.y));
        }
        public static Vector2 Ceil(this Vector2 v){
            return new Vector2(Mathf.Ceil(v.x),Mathf.Ceil(v.y));
        }
        public static Vector2 Reflect(this Vector2 v, Vector2 norm){
            return Vector2.Reflect(v,norm);
            // return (v - (norm * 2 * Vector2.Dot(v,norm)));
        }
        public static Vector2 Bounce(this Vector2 v, Vector2 norm){
            return -(Vector2.Reflect(v,norm));
        }
        public static Vector2 Project(this Vector2 v, Vector2 norm){
            return (norm * MY.SafeDivide(Vector2.Dot(v,norm), norm.LengthSquared()));
        }
        public static Vector2 Slide(this Vector2 v, Vector2 other){
            return (v - (other * Vector2.Dot(v,other)));
        }

        public static float Gradient(this Vector2 v){
            return MY.SafeDivide(v.y, v.x);
        }

        public static Vector2 ClosestPoint(this Vector2 v,Vector2[] arr, bool exclusive=false){
            Vector2 pt = Vector2.positiveInfinity;
            float dist = float.PositiveInfinity;

            foreach(Vector2 vec in arr){
                if(vec == v && exclusive) continue;
                var _dist = Mathf.Abs(vec.DistanceTo(v));
                if(_dist < dist){
                    pt = vec; dist = _dist;
                }
            }
            return pt;
        }
        public static Vector2[] SortPointsByClosest(this Vector2 v, Vector2[] arr){
            var arrList = new List<Vector2>(arr);

            int len = arrList.Count;
            var newList = new List<Vector2>();

            for(int i=0;i<len;i++){
                var pt = v.ClosestPoint(arrList.ToArray());
                if(pt == Vector2.positiveInfinity) continue;

                newList.Add(pt);
                arrList.Remove(pt);
                i--;
            }
            return newList.ToArray();
        }
        public static Vector2 QuadraticBezier(Vector2[] pts, float t){
            var qPoints = new List<Vector2>();

            for(int i=0;i<pts.Length-1;i++){
                var pA = pts[i]; var pB = pts[i+1];

                var pt = pA.Lerp(pB, t);

                qPoints.Add(pt);
            }

            if(qPoints.Count<2) return qPoints[0];

            return Vector2Ext.QuadraticBezier(qPoints.ToArray(),t);
        }

        public static Vector2 QuadraticBezier(Vector2 ptA, Vector2 ptB, Vector2 ptC, float t){
            return Vector2Ext.QuadraticBezier(new Vector2[]{
                ptA, ptB, ptC
            }, t);
        }
        public static Vector2 CubicBezier(Vector2 ptA, Vector2 ptB, Vector2 ptC, Vector2 ptD, float t){
            return Vector2Ext.QuadraticBezier(new Vector2[]{
                ptA, ptB, ptC, ptD
            }, t);
        }

        public static Vector2[] QuadraticBezierPoints(Vector2[] pts, float inc){
            var qPoints = new List<Vector2>();
            if(inc>0){
                float t = 0; bool last = false;
                while(t <= 1){
                    qPoints.Add( Vector2Ext.QuadraticBezier(pts,t) );
                    t += inc;
                    if(t>1 && !last){
                        t = 1; last = true;
                    }
                }
            }

            return qPoints.ToArray();
        }

        public static float[] AsArray(this Vector2 v){
            return new float[]{v.x,v.y};
        }
        public static Dictionary<string,float> AsDictionary(this Vector2 v){
            return new Dictionary<string, float>{
                {"x",v.x}, {"y",v.y}
            };
        }

    } 
    // public class Vector2 : UnityEngine.Vector2 {

    // }
	[Serializable]
    public class Vector2Line {
        public float a, b, c;

        public static Vector2Line YAxis{
            get {
                return new Vector2Line(1,0,0);
            }
        }
        public static Vector2Line XAxis{
            get {
                return new Vector2Line(0,-1,0);
            }
        }
        public static Vector2Line One{
            get {
                return new Vector2Line(1,-1,0);
            }
        }
        public static Vector2Line NegativeOne{
            get {
                return new Vector2Line(1,1,0);
            }
        }

        public float Gradient{
            get {
                return MY.SafeDivide(- a, b);
            }
        }public float m{
            get { return Gradient;}
        }
        public float XIntercept{
            get {
                return MY.SafeDivide(- c, a);
            }
        }public float f{
            get { return XIntercept;}
        }
        public float YIntercept{
            get {
                return MY.SafeDivide(- c, b);
            }
        }public float e{
            get { return YIntercept;}
        }

        public Vector2Line(Vector2 v1, Vector2 v2){
            var vX = (v1 - v2);
            var _m = MY.SafeDivide(vX.y, vX.x);

            if(float.IsInfinity(_m)){
                //vertical
                this.c = -((v1.x != 0) ? v1.x : v2.x);
                this.b = 0;
                this.a = 1;
            }else if(_m == 0){
                //horizontal
                this.c = ((v1.y != 0) ? v1.y : v2.y);
                this.b = -1;
                this.a = 0;
            }else{
                var _e = (v1.y) - (_m * (v1.x));
                var _f = MY.SafeDivide(-_e, _m);

                this.c = - (_f * _m);
                this.b = MY.SafeDivide(-this.c, _e);
                this.a = - (this.b * _m);
            }
        }

        public Vector2Line(float xIntOrGradient, float yInt, bool useIntercepts = false){
            float _f, _e, _m;
            if(useIntercepts){
                _m = MY.SafeDivide(-yInt, xIntOrGradient);
                _e = yInt;
                _f = xIntOrGradient;

                this.c = -(_f * _m);
                this.b = MY.SafeDivide(-this.c, _e);
                this.a = - (this.b * _m);
            }else{
                _m = xIntOrGradient;
                _e = yInt;

                if(float.IsInfinity(_m)){
                    //treat _e as _f since theres no y intercept
                    this.c = -_e;
                    this.b = 0;
                    this.a = 1;
                }else if(_m == 0){
                    this.c = _e;
                    this.b = -1;
                    this.a = 0;
                }else{
                    _f = MY.SafeDivide(-_e, _m);

                    this.c = - (_f * _m);
                    this.b = MY.SafeDivide(-this.c, _e);
                    this.a = - (this.b * _m);
                }
                
            }
            
            
        }
        
        public Vector2Line(float a, float b, float c){
            this.c = c;
            this.b = b;
            this.a = a;
        }

        public Vector2Line Clone(){
            return new Vector2Line(a,b,c);
        }

        public float GetX(float y){
            if(b != 0){
                return MY.SafeDivide((y - e), m);
            }else{
                return MY.SafeDivide(-c, a);
            }
        }
        public float GetY(float x){
            if(a != 0){
                return (m * x) + e;
            }else{
                return MY.SafeDivide(-c, b);
            }
        }

        public bool Equals(Vector2Line other){
            return (
                Gradient == other.Gradient &&
                YIntercept == other.YIntercept &&
                XIntercept == other.XIntercept
            );
        }

        public bool IsHorizontal(){ return (a == 0);}
        public bool IsVertical(){ return (b == 0);}

        public bool HasPoint(Vector2 v){
            float res;
            if(float.IsInfinity(a) && float.IsInfinity(b)){
                if(a == b){
                    res = float.PositiveInfinity + c;
                }else{
                    res = 0 + c;
                }
            }else{
               res = ((a*v.x) + (b*v.y) + c); 
            }

            return (res == 0);
        }

        public float getAngle(){
            if(XIntercept == float.PositiveInfinity)
                return 0;
            if(XIntercept == float.NegativeInfinity)
                return (float) Math.PI;
            if(YIntercept == float.PositiveInfinity)
                return (float) Math.PI/2;
            if(YIntercept == float.NegativeInfinity)
                return (float) - Math.PI/2;
            
            var vX = new Vector2(XIntercept,0);
            var vY = new Vector2(0, YIntercept);

            float res = Vector2.Angle((vY-vX),Vector2.zero);
            return res;

        }

        public Vector2 Intersect(Vector2Line other){
            //x = (b1c2-b2c1)/(a1b2-a2b1) = BC/AB
		    //y = (a2c1-a1c2)/(a1b2-a2b1) = AC/AB

            if(other.Gradient == Gradient){
                return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            }

            var a1 = this.a; var a2 = other.a;
		    var b1 = this.b; var b2 = other.b;
		    var c1 = this.c; var c2 = other.c;

            var b1c2 = b1 * c2; var b2c1 = b2 * c1;
		    var a1b2 = a1 * b2; var a2b1 = a2 * b1;
		    var a2c1 = a2 * c1; var a1c2 = a1 * c2;

            float BC, AB, AC;

            if(float.IsInfinity(b1c2) && float.IsInfinity(b2c1) && b1c2==b2c1){
                BC = b1c2;
            }else{
                BC = (b1c2-b2c1);
            }
            if(float.IsInfinity(a1b2) && float.IsInfinity(a2b1) && a1b2==a2b1){
                AB = a1b2;
            }else{
                AB = (a1b2-a2b1);
            }
            if(float.IsInfinity(a2c1) && float.IsInfinity(a1c2) && a2c1==a1c2){
                AC = a2c1;
            }else{
                AC = (a2c1-a1c2);
            }

            var x = MY.SafeDivide(BC,AB);
            var y = MY.SafeDivide(AC,AB);

            return new Vector2(x,y);
        }

        public Vector2Line Perpendicular(Vector2 pt){
            if(a == 0){
                return new Vector2Line(b,a,pt.x);
            }else if(b == 0){
                return new Vector2Line(b,a,pt.y);
            }else{
                var grad = MY.SafeDivide(-1, Gradient);
                var yInt = (pt.y) + ((1/grad)*pt.x);
                var xInt = MY.SafeDivide(-yInt, grad);

                if(float.IsInfinity(xInt)){
                    return new Vector2Line(grad,yInt,false);
                }else{
                    return new Vector2Line(xInt, yInt, true);
                }
            }
        }

        public Vector2 Normal(){
            return new Vector2(a,b);
        }

        public Vector2 Mirror(Vector2 pt){
            if(HasPoint(pt)){
                return new Vector2(pt.x,pt.y);
            }

            var normal = Normal();
            var unitNormal = normal.normalized;

            var unitC = MY.SafeDivide(c, normal.magnitude);
            
            var signedDist = (unitNormal.x * pt.x) + (unitNormal.y * pt.y) + unitC;

            var mx = pt.x - 2 * unitNormal.x * signedDist;
            var my = pt.y - 2 * unitNormal.y * signedDist;

            return new Vector2(mx,my);
        }

    }

	[Serializable]
	public class Rect2D{
		public Vector2 position = Vector2.zero;
		public Vector2 size = Vector2.zero;

		public static Rect2D origin{
			get{
				return new Rect2D(0,0,1,1);
			}
		}
		public static Rect2D one{
			get{
				return new Rect2D(1,1,1,1);
			}
		}
		public static Rect2D zero{
			get{
				return new Rect2D(0,0,0,0);
			}
		}

		public static bool EQUALS(Rect2D r1, Rect2D r2){
			if(r1 == r2) return true;
			if(r1==null || r2==null) return false;

			if( 
				r1.position == r2.position &&
				r1.size == r2.size
			) return true;

			return false;
		}

		public static Rect2D COMBINE(Rect2D[] rects){
			var pts = new List<Vector2>();
			foreach(var r in rects){
				var corners = r.GetCorners();
				foreach(var pt in corners){
					pts.Add(pt);
				}
			}
			
			return Rect2D.From(pts.ToArray());
		}

		public static Rect2D From(Vector2[] pts){
			float xMin, xMax, yMin, yMax;
			xMax = yMax = float.NegativeInfinity;
			xMin = yMin = float.PositiveInfinity;

			foreach(var pt in pts){
				if(xMin>pt.x) xMin = pt.x;
				if(yMin>pt.y) yMin = pt.y;
				if(xMax<pt.x) xMax = pt.x;
				if(yMax<pt.y) yMax = pt.y;
			}

			var w = xMax - xMin;
			var h = yMax - yMin;

			return new Rect2D(xMin,yMin,w,h);
		}

		public Vector2 start{
			get{ return new Vector2(xMin,yMin); }
		}
		public Vector2 end{
			get{ return new Vector2(xMax,yMax); }
		}
		public Vector2 center{
			get{ return Vector2.Lerp(start,end,0.5f); }
		}
		public Vector2 extents{
			get{ return (center - start); }
		}

		public Vector2 min{
			get{ return new Vector2(xMin,yMin); }
		}
		public Vector2 maxMin{
			get{ return new Vector2(xMax,yMin); }
		}
		public Vector2 minMax{
			get{ return new Vector2(xMin,yMax); }
		}
		public Vector2 max{
			get{ return new Vector2(xMax,yMax); }
		}

		public float x{
			get{ return position.x; }
			set{ position.x = value; }
		}
		public float y{
			get{ return position.y; }
			set{ position.y = value; }
		}
		public float w{
			get{ return size.x; }
			set{ size.x = value; }
		}
		public float h{
			get{ return size.y; }
			set{ size.y = value; }
		}

		public float width{
			get{ return w; } set{ w = value; }
		}
		public float height{
			get{ return h; } set{ h = value; }
		}

		public float xMin{
			get{ return x; }
			set{
				w = (xMax - value);
				x = value;
			}
		}
		public float yMin{
			get{ return y; }
			set{
				w = (yMax - value);
				y = value;
			}
		}
		public float xMax{
			get{ return x + w; }
			set{
				w = (value - xMin);
			}
		}
		public float yMax{
			get{ return y + h; }
			set{
				h = (value - yMin);
			}
		}

		public Rect2D(float x, float y, float w, float h){
			position = new Vector2(x,y);
			size = new Vector2(w,h);
		}
		public Rect2D(Vector2 p, Vector2 s){
			position = p;
			size = s;
		}
		public Rect2D(){

		}
		public Rect2D(Rect2D other){
			position = other.position;
			size = other.size;
		}

		public bool Equals(Rect2D other){
			return Rect2D.EQUALS(this,other);
		}

		public Rect2D Abs(){
			return new Rect2D(position, size.Abs());
		}

		public bool ContainsPoint(Vector2 v){
			if(
				(v.x < this.xMin) ||
				(v.x > this.xMax) ||
				(v.y < this.yMin) ||
				(v.y > this.yMax)
			)
				return false;
			return true;
		}
		public Rect2D GetIntersectWith(Rect2D other, float threshold=0){
			if(!this.IntersectsWith(other,threshold))
				return null;

			var yMin = (this.yMin > other.yMin)?this.yMin:other.yMin;
			var yMax = (this.yMax < other.yMax)?this.yMax:other.yMax;

			var xMin = (this.xMin > other.xMin)?this.xMin:other.xMin;
			var xMax = (this.xMax < other.xMax)?this.xMax:other.xMax;

			return new Rect2D(
				new Vector2(xMin,yMin), new Vector2(xMax-xMin, yMax-yMin)
			);
		}
		public bool IntersectsWith(Rect2D other, float threshold=0){
			if(
				(this.xMax + threshold) < (other.xMin) ||
				(this.xMin - threshold) > (other.xMax) ||
				(this.yMax + threshold) < (other.yMin) ||
				(this.yMin - threshold) > (other.yMax)
			)
				return false;
			return true;
		}
		public bool IsTouching(Rect2D other, float threshold=0){
			return this.Touches(other,threshold);
		}
		public bool Touches(Rect2D other, float threshold=0){
			if(
				(this.xMax + threshold) == (other.xMin) ||
				(this.xMin - threshold) == (other.xMax) ||
				(this.yMax + threshold) == (other.yMin) ||
				(this.yMin - threshold) == (other.yMax)
			)
				return true;
			return false;
		}

		public Rect2D Combine(Rect2D other){
			return Rect2D.COMBINE(new Rect2D[]{this, other});
		}
		public Vector2[] GetCorners(){
			return new Vector2[]{min, maxMin, max, minMax};
		}
		public Vector2[] ClampPoints(Vector2[] pts){
			var rect = this;
			var newPts = new List<Vector2>();
			foreach(var _pt in pts){
				Vector2 pt = _pt;
				if(pt.y > rect.yMax) pt.y = rect.yMax;
				if(pt.x > rect.xMax) pt.x = rect.xMax;
				if(pt.y < rect.yMin) pt.y = rect.yMin;
				if(pt.x < rect.xMin) pt.x = rect.xMin;
				newPts.Add(pt);
			}
			return newPts.ToArray();
		}
		public float[] ToArray(){
			return new float[]{x,y,w,h};
		}

	}

	[Serializable]
	public class Transform2D{

		public Vector2 position = Vector2.zero;
		public float rotation = 0f;
		public Vector2 scale = Vector2.one;
		public Vector2 skew = Vector2.zero;
		public Vector2 anchor = Vector2.zero;

		public Transform2D parent = null;
		protected List<Transform2D> childs = new List<Transform2D>();

		public static Transform2D origin{
			get{
				return new Transform2D();
			}
		}

		public static bool EQUALS(Transform2D t1, Transform2D t2){
			if(t1 == t2) return true;
			if(t2==null || t2==null) return false;

			if(
				t1.position == t2.position &&
				t1.scale == t2.scale &&
				t1.skew == t2.skew &&
				t1.anchor == t2.anchor &&
				t1.rotation == t2.rotation
			) return true;

			return false;
		}
		public static bool SIMILAR(Transform2D t1, Transform2D t2){
			if(t1 == t2) return true;
			if(t2==null || t2==null) return false;

			if(
				t1.position == t2.position &&
				t1.scale == t2.scale &&
				t1.skew == t2.skew &&
				t1.anchor == t2.anchor &&
				t1.rotation == t2.rotation
			) return true;

			return false;
		}

		public static Transform2D INVERSE(Transform2D t){
			var p = -t.position;
			var s = t.scale.Inverse();
			var r = -(t.rotation);
			var k = -t.skew;
			var a = t.anchor;

			return new Transform2D(p,r,s,k,a); 
		}

		public Transform2D[] children{
			get{
				return childs.ToArray();
			}
		}

		public float a{
			get{
				return (this.scale.x) * ( Mathf.Cos(this.rotation) - (Mathf.Sin(this.rotation) * Mathf.Tan(this.skew.x)) );
			}	
		}
		public float b{
			get{
				return (this.scale.y) * ( Mathf.Sin(this.rotation) + (Mathf.Cos(this.rotation) * Mathf.Tan(this.skew.y)) );
			}
		}
		public float c{
			get{
				return (this.scale.x) * ( (Mathf.Cos(this.rotation) * Mathf.Tan(this.skew.x)) - Mathf.Sin(this.rotation) );
			}
		}
		public float d{
			get{
				return (this.scale.y) * ( (Mathf.Sin(this.rotation) * Mathf.Tan(this.skew.y)) + Mathf.Cos(this.rotation) );
			}
		}
		public float tx{
			get{
				return this.position.x;
			}
		}
		public float ty{
			get{
				return this.position.y;
			}
		}

		public float[][] matrix{
			get{
				return new float[][]{
					new float[]{this.a, this.c, this.tx},
					new float[]{this.b, this.d, this.ty},
					new float[]{0,0,1}
				};
			}
		}

		public Transform2D(){

		}
		public Transform2D(Vector2 position, float rotation, Vector2 scale, Vector2 skew, Vector2 anchor){
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
			this.skew = skew;

			this.anchor = anchor;
		}
		public Transform2D(Transform2D other){
			position = other.position;
			rotation = other.rotation;
			scale = other.scale;
			skew = other.skew;

			anchor = other.anchor;
		}

		public Transform2D Inverted(){
			return Transform2D.INVERSE(this);
		}

		public void SetParent(Transform2D x){
			if(x == this) return;

			if(parent != null){
				parent.RemoveChild(this);
			}
			parent = x;
		}

		public void AddChild(Transform2D x){
			if(x == this) return;
			
			x.SetParent(this);
			childs.Add(x);
		}
		public Transform2D RemoveChild(Transform2D x){
			if(x == this) return null;

			int ind = childs.IndexOf(x);
			Transform2D child = this.childs[ind];
			this.childs.Remove(x);
			child.parent = null;

			return child;
		}
		public Transform2D RemoveChild(int x){
			if(x<0 || x>=childs.Count) return null;

			Transform2D child = this.childs[x];
			this.childs.RemoveAt(x);
			child.parent = null;

			return child;
		}

		public Transform2D GetGlobalTransform(){
			var parentTransform = this.parent?.GetGlobalTransform();
			if(parentTransform==null){
				parentTransform = Transform2D.origin;
			}
			var pt = parentTransform;
			

			var p = (pt.position + this.position);
			var r = pt.rotation + this.rotation;
			var s = (pt.scale * this.scale);
			var k = new Vector2(
				Mathf.Tan( Mathf.Atan(this.skew.x) + Mathf.Atan(pt.skew.x) ) ,
				Mathf.Tan( Mathf.Atan(this.skew.y) + Mathf.Atan(pt.skew.y) )
			);
			var a = pt.ApplyTransform(this.anchor, pt.anchor);

			return new Transform2D(p,r,s,k,a);
		}

		public Vector2 ApplyGlobalTransform(Vector2 pt){
			return ApplyGlobalTransform(pt, this.anchor, new string[]{"S","K","R","T"});
		}
		public Vector2 ApplyGlobalTransform(Vector2 pt, Vector2 anchor){
			return ApplyGlobalTransform(pt, anchor, new string[]{"S","K","R","T"});
		}
		public Vector2 ApplyGlobalTransform(Vector2 pt, Vector2 anchor, string[] order){
			var globalTrans = this.GetGlobalTransform();
			var newPt = pt;

			newPt = globalTrans.ApplyTransform(pt, anchor, order);

			return newPt;
		}

		public Vector2 ApplyTranslate(Vector2 pt, Vector2 anchor){
			var newPt = pt;
			newPt = (newPt - anchor);
			newPt = (newPt + this.position);
			newPt = (newPt + anchor);
			return newPt;
		}
		public Vector2 ApplyTranslate(Vector2 pt){
			return ApplyTranslate(pt, this.anchor);
		}
		public Vector2 ApplyRotate(Vector2 pt, Vector2 anchor){
			var newPt = pt;
			newPt = (newPt - anchor);
			newPt = newPt.Rotated(Vector2.zero, this.rotation);
			newPt = (newPt + anchor);
			return newPt;
		}
		public Vector2 ApplySkew(Vector2 pt, Vector2 anchor){
			var newPt = pt;
			newPt = (newPt - anchor);
			newPt = newPt.Skew(Vector2.zero, this.skew);
			newPt = (newPt + anchor);
			return newPt;
		}
		public Vector2 ApplyScale(Vector2 pt, Vector2 anchor){
			var newPt = pt;
			newPt = (newPt - anchor);
			newPt = (newPt * this.scale);
			newPt = (newPt + anchor);
			return newPt;
		}

		public Vector2 ApplyInverseTransform(Vector2 pt, Vector2 anchor, string[] order){
			var inv = this.Inverted();

			return inv.ApplyTransform(pt, anchor, order);
		}
		public Vector2 ApplyInverseTransform(Vector2 pt){
			return ApplyInverseTransform(pt, this.anchor, new string[]{"T","R","K","S"});
		}
		public Vector2 ApplyInverseTransform(Vector2 pt, Vector2 anchor){
			return ApplyInverseTransform(pt, anchor, new string[]{"T","R","K","S"});
		}

		public Vector2 ApplyTransform(Vector2 pt){
			return ApplyTransform(pt, this.anchor, new string[]{"S","K","R","T"});
		}
		public Vector2 ApplyTransform(Vector2 pt, Vector2 anchor){
			return ApplyTransform(pt, anchor, new string[]{"S","K","R","T"});
		}
		public Vector2 ApplyTransform(Vector2 pt, Vector2 anchor, string[] order){
			var newPt = pt;
			newPt = (newPt - anchor);
			foreach(var trans in order){
				if(trans==null) continue;

				switch(trans.ToUpper()){
					case "T":case "TRANSLATE":case "POSITION":
						newPt = this.ApplyTranslate(newPt, Vector2.zero);
						break;
					case "R":case "ROTATE":case "ROTATION":
						newPt = this.ApplyRotate(newPt, Vector2.zero);
						break;
					case "K":case "SKEW":
						newPt = this.ApplySkew(newPt, Vector2.zero);
						break;
					case "S":case "SCALE":case "SIZE":
						newPt = this.ApplyScale(newPt, Vector2.zero);
						break;
					default:
						// newPt = newPt;
						break;
				}
			}
			newPt = (newPt + anchor);

			return newPt;
		}

		public float[] ToArray(){
			return new float[]{this.a, this.b, this.c, this.d, this.tx, this.ty};
		}

	}
}