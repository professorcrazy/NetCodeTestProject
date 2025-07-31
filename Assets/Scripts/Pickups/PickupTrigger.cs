using System.Runtime.CompilerServices;
using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
    [SerializeField] private BoosterEffectSO boostereffect;
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) { 
            return; 
        }
        boostereffect.Apply(other.gameObject);
        Destroy(this.gameObject);
    }
}
