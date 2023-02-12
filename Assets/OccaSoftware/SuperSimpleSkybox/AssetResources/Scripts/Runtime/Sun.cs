using UnityEngine;

namespace OccaSoftware.SuperSimpleSkybox.Runtime
{
	[AddComponentMenu("SuperSimpleSkybox/Sun")]
	public class Sun : DirectionalLight
    {
		protected override void Update()
		{
			base.Update();
			Shader.SetGlobalVector(_SunDirection, -transform.forward);
		}
		private static int _SunDirection = Shader.PropertyToID("_SunDirection");
	}
}
