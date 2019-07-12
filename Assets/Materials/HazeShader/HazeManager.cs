using System.Collections.Generic;
using UnityEngine.Rendering;

public class HazeManager {

    #region Singleton

    private static HazeManager _instance;

    public static HazeManager Instance {
        get { return _instance = _instance ?? new HazeManager(); }
    }

    #endregion

    private readonly List<HazeEffect> _hazeEffects = new List<HazeEffect>();

    public void Register(HazeEffect hazeEffect) { _hazeEffects.Add(hazeEffect); }

    public void Deregister(HazeEffect hazeEffect) { _hazeEffects.Remove(hazeEffect); }

    public void PopulateCommandBuffer(CommandBuffer commandBuffer) {

        for (int i = 0, len = _hazeEffects.Count; i < len; i++) {

            var effect = _hazeEffects[i];
            commandBuffer.DrawRenderer(effect.Renderer, effect.Material);
        }
    }
}