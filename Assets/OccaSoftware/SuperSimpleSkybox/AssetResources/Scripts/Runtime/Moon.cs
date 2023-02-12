using UnityEngine;

namespace OccaSoftware.SuperSimpleSkybox.Runtime
{
	[AddComponentMenu("SuperSimpleSkybox/Moon")]
	public class Moon : DirectionalLight
	{
		protected override void Update()
		{
			base.Update();
			Shader.SetGlobalVector(_MoonDirection, -transform.forward);
		}
		private static int _MoonDirection = Shader.PropertyToID("_MoonDirection");
	}
}
