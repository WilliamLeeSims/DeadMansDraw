using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Linq;

namespace BUtility
{
  using UnityEngine;

  public static class EnumUtil
  {
    public static IEnumerable<T> GetValues<T>()
    {
      return Enum.GetValues( typeof(T) ).Cast<T>();
    }
  };

  public static class RectTransformUtil
  {
    public static bool IsInside( this RectTransform rt, Vector2 screenPos, Camera eventCamera )
    {
      var isInside = RectTransformUtility.RectangleContainsScreenPoint(
          rt, screenPos, eventCamera );

      if( isInside )
      {
        var colliders = rt.GetComponentsInChildren<Collider2D>();
        if( colliders.Length > 0 )
        {
          isInside = false;
          foreach( var coll in colliders )
            isInside |= coll.OverlapPoint( screenPos );
        }
      }

      return isInside;
    }
  }

  public static class MathUtil
  {
    public static float TAU = Mathf.PI * 2;
    public static Vector3 CylindricalCoordinates( float radius, float angDegrees, float z, Vector3 offset = default(Vector3) )
    {
      return new Vector3( radius * Mathf.Cos( Mathf.Deg2Rad * angDegrees ) + offset.x, radius * Mathf.Sin( Mathf.Deg2Rad * angDegrees ) + offset.y, z + offset.z );
    }

    public static Vector3 piecewiseMin( params Vector3[] m )
    {
      return new Vector3( m.Min( t => t.x ), m.Min( t => t.y ), m.Min( t => t.z ) );
    }
    public static Vector3 piecewiseMax( params Vector3[] m )
    {
      return new Vector3( m.Max( t => t.x ), m.Max( t => t.y ), m.Max( t => t.z ) );
    }

    public static int PosMod( this int i, int n ) { if( n < 0 ) n = -n; int rv = i % n; return rv < 0 ? rv + n : rv; }

    public static int RandomSign { get { return UnityEngine.Random.value < 0.5f ? -1 : 1; } }
    public static bool RandomBool { get { return UnityEngine.Random.value < 0.5f; } }

    public static int RandomRange( int exclusiveMax ) { return UnityEngine.Random.Range( 0, exclusiveMax ); }
    public static int RandomPlusMinus( int inclusiveMax ) { return RandomSign * (1 + RandomRange(inclusiveMax)); }

    public static bool Between<T>( this T x, T inclusiveMin, T inclusiveMax ) where T:IComparable
    {
      return inclusiveMin.CompareTo( x ) <= 0 && x.CompareTo( inclusiveMax ) <= 0;
    }

    public static float Hacovercosine( float radians )
    {
      return 0.5f * (1f + Mathf.Sin( radians ));
    }

    public static Vector3 Rotate( this Vector3 v, float angleDegrees )
    {
      return Quaternion.Euler( 0, 0, angleDegrees ) * v;
    }

    public static string ToHex( this Color color )
    {
      return
        Mathf.RoundToInt( 255 * color.r ).ToString( "X2" ) +
        Mathf.RoundToInt( 255 * color.g ).ToString( "X2" ) +
        Mathf.RoundToInt( 255 * color.b ).ToString( "X2" );
    }
  }

  public static class ListUtil
  {
    public static string ShuffleString( this string s )
    {
      return new string( s.ToCharArray().OrderBy( c => UnityEngine.Random.Range( 0, 2 ) == 0 ).ToArray() );
    }
    public static string Label( this int v, string singular, string plural )
    {
      return v + (v == 1 ? singular : plural);
    }

    public static TValue GetValueOrDefault<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue )
    {
      TValue value;
      return dictionary.TryGetValue( key, out value ) ? value : defaultValue;
    }

    public static bool In<T>( this T x, params T[] set )
    {
      return set.Contains( x );
    }

    public static T Switch<T>( this int index, params T[] set )
    {
      if( index < 0 )
        return set.Length > 0 ? set[0] : default( T );
      if( index >= set.Length )
        return set.Length > 0 ? set[set.Length - 1] : default( T );
      return set[index];
    }

    public static IEnumerable<T[]> GetPermutations<T>( T[] values )
    {
      if( values.Length == 1 )
        return new[] { values };

      return values.SelectMany( v => GetPermutations( values.Except( new[] { v } ).ToArray() ),
          ( v, p ) => new[] { v }.Concat( p ).ToArray() );
    }

    public static void ForEach<T>( this IEnumerable<T> ie, Action<T, int> action )
    {
      var i = 0;
      foreach( var e in ie )
        action( e, i++ );
    }
    public static void ForEach<T>( this T[] ie, Action<T, int> action )
    {
      var i = 0;
      foreach( var e in ie )
        action( e, i++ );
    }
  }
}
