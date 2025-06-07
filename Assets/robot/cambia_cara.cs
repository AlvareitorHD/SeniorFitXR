using UnityEngine;

public class CambiarAlbedoCara : MonoBehaviour
{
    // Material de la cara (no el renderer)
    public Material caraMaterial;
    public Texture texturaNormal;
    public Texture texturaFeliz;
    public Texture texturaEsfuerzo;
    public Texture texturaAparicion;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("armature|Action_saludo") || stateInfo.IsName("Bienvenida"))
        {
            caraMaterial.SetTexture("_MainTex", texturaFeliz);
        }
        else if (stateInfo.IsName("armature|Action_señala"))
        {
            caraMaterial.SetTexture("_MainTex", texturaNormal);
        }
        else if (stateInfo.IsName("Yoga") || stateInfo.IsName("AlzarBrazos"))
        {
            caraMaterial.SetTexture("_MainTex", texturaEsfuerzo);
        }
        else if(stateInfo.IsName("armature|Action_aparicion") || stateInfo.IsName("Aparicion"))
        {
            caraMaterial.SetTexture("_MainTex", texturaAparicion);
        }
        else
        {
            caraMaterial.SetTexture("_MainTex", texturaNormal);
        }
    }
}
