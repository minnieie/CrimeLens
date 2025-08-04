using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    MeshRenderer myMeshRenderer;

    [SerializeField]
    Material highlightMat; 
    Material originalMat;
    public int coinValue = 10;

    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        originalMat = myMeshRenderer.material; // Store the original material
    }

    public void Highlight()
    {
        myMeshRenderer.material = highlightMat; // Change to highlight material
        
    }

    public void Unhighlight()
    {
        myMeshRenderer.material = originalMat; // Reset to original material
    }

    public void Collect(PlayerBehaviour player)
    {
        GameManager.instance.ModifyScore(coinValue); 
        Destroy(gameObject);
    }
}