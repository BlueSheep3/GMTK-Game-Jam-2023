using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
class CustomRuleTile : RuleTile
{
	public Type type = Type.Normal;

	public enum Type : byte {
		Normal, Slippery, Sticky
	}


	public override bool RuleMatch(int neighbor, TileBase other)
	{
		if(other is RuleOverrideTile ruleOverrideTile)
			other = ruleOverrideTile.m_InstanceTile;

		return neighbor switch {
			1 => other == this || other is CustomRuleTile,
			2 => other != this && other is not CustomRuleTile,
			_ => true,
		};
	}
}