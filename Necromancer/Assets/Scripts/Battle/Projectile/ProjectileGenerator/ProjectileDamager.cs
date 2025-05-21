using UnityEngine;

public class ProjectileDamager : MonoBehaviour
{

    private BaseProjectileGenerator generator;

    public void Initialize(BaseProjectileGenerator _generator)
    {
        generator = _generator;
    }

    public void ApplyDamage(Transform transform)
    {
        
        

    }
}
