using UnityEngine;

public class AnimChangerLayer : MonoBehaviour
{
    AnimationClip myAnimationClip;
    protected Animator animator;
    protected AnimatorOverrideController animatorOverrideController;

    public bool multiSimulation;

    [SerializeField]
    public enum Layer
    {
        Base,
        Superior,
        Inferior
    }
    [SerializeField] public Layer myLayer = Layer.Base;

    [SerializeField]
    public enum Identidad
    {
        Agente,
        Geco,
        Raton,
        Gorrion
    }
    [SerializeField] public Identidad yo;// = Identidad.Agente;
    public string strPlayAnim = "";
    string strPreviousAnim = "", strLayerID, ruta;
    int ntParity = -1;
    string[] strEnumIdentidad = System.Enum.GetNames(typeof(Identidad));

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = !multiSimulation;

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        ruta = "Animations/" + strEnumIdentidad[(int)yo] + "/"; //print(ruta);
    }

    void LateUpdate()
    {
        if(!multiSimulation && !strPlayAnim.Equals(strPreviousAnim)) MainChanger();
    }

    AnimationClip SetClip(string path) { return (myAnimationClip = Resources.Load<AnimationClip>(path)); }
    void PlayAnim(string strAnim) { animatorOverrideController[strLayerID] = SetClip(ruta + strAnim); }
    void StopAnim() { animatorOverrideController[strLayerID] = SetClip(ruta+"Idle"); }

    void SetLayer()
    {
        ntParity++;
        switch(myLayer)
        {
            case Layer.Base:
                if (ntParity % 2 == 0) 
                {
                    strLayerID = "Idle";
                    animator.SetBool("BaseTransition", false);
                }
                else{
                    strLayerID = "Transition";
                    animator.SetBool("BaseTransition", true);
                }
                CambiarPeso(true);
                break;
            case Layer.Superior: strLayerID = "Idle 1"; CambiarPeso(false); break;
            case Layer.Inferior: strLayerID = "Idle 2"; CambiarPeso(false); break;
        }
    }

    void CambiarPeso(bool isBase)
    {
        float peso = isBase ? 0f : 0.75f;

        animator.SetLayerWeight((int)Layer.Superior, peso);
        animator.SetLayerWeight((int)Layer.Inferior, peso);
    }

    void MainChanger()
    {
        StopAnim();
        SetLayer();
        PlayAnim(strPlayAnim);
        strPreviousAnim = strPlayAnim;
    }

    public void Animar(string strAnim, Layer layer)
    {
        myLayer = layer;
        strPlayAnim = strAnim;
    }

}
