using UnityEngine;

public class CambiarAlbedoCara : MonoBehaviour
{
    // Material de la cara (no el renderer)
    public Material caraMaterial;
    public Texture texturaNormal;
    public Texture texturaFeliz;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Armature_001|Action_saludo"))
        {
            caraMaterial.SetTexture("_MainTex", texturaFeliz);
        }
        else if (stateInfo.IsName("Armature_001|Action_señala"))
        {
            caraMaterial.SetTexture("_MainTex", texturaNormal);
        }
    }
}
