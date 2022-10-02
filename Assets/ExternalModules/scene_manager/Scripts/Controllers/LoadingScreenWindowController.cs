using System.Threading.Tasks;
using UnityEngine;

namespace versoft.scene_manager
{
    public class LoadingScreenWindowController : WindowController
    {
        [SerializeField]
        private Animation animationComponent;

        public async Task FadeIn()
        {
            if (animationComponent == null)
            { return; }

            var clip = animationComponent.GetClip("FadeIn");
            if (clip == null)
            { return; }

            animationComponent.Play(clip.name);
            await Task.Delay(Mathf.RoundToInt(clip.length * 1000));
        }

        public async Task FadeOut()
        {
            if (animationComponent == null)
            { return; }

            var clip = animationComponent.GetClip("FadeOut");
            if (clip == null)
            { return; }

            animationComponent.Play(clip.name);
            await Task.Delay(Mathf.RoundToInt(clip.length * 1000));
        }
    }
}
