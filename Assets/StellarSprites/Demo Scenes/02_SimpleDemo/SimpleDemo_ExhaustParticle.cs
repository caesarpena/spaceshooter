using UnityEngine;

public class SimpleDemo_ExhaustParticle : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Texture2D texture = new Texture2D(16, 16);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float a = Vector2.Distance(new Vector2(x, y), new Vector2(texture.width / 2, texture.height / 2));

                Color c = new Color(a, a, a);
                texture.SetPixel(x, y, c);
            }
        }

        texture.Apply();

        GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find("Particles/Additive");
        GetComponent<ParticleSystemRenderer>().material.mainTexture = texture;
        GetComponent<ParticleSystemRenderer>().sortingLayerName = transform.parent.GetComponent<SpriteRenderer>().sortingLayerName;
        GetComponent<ParticleSystemRenderer>().sortingOrder = -1;
    }
}
