using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class HazeEffect : MonoBehaviour {

    public Renderer Renderer { get; private set; }

    public Material Material { get; private set; }

    private void OnEnable() {
        Renderer = GetComponent<Renderer>();
        Renderer.enabled = false;
        Material = Renderer.sharedMaterial;
        HazeManager.Instance.Register(this);
    }

    private void OnDisable() {
        HazeManager.Instance.Deregister(this);
    }
}