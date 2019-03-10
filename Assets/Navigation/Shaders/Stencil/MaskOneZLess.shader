Shader "Custom/Stencil/Mask OneZLess"
{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	SubShader{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry-1"  }
		ColorMask 0
		ZWrite off
		
		Pass {
			Stencil {
				Ref 1
				Comp always
				Pass Replace
			}
		}

		UsePass "Transparent/Diffuse/FORWARD"
	}
}