using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeScript : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 initialPos;

    public float highestShake;
    public IEnumerator shaker;

    List<shakeInfo> shakes = new List<shakeInfo>();

    void Start()
    {
        target = GetComponent<Transform>();
        initialPos = target.localPosition;
    }

    public void ShakeAdd(float intensity, float duration){
        if(intensity != 0 || duration != 0){
            shakeInfo newShake = new shakeInfo();
            newShake.shakeIntensity = intensity;
            newShake.shakeDuration = duration;
            shakes.Add(newShake);
            StartCoroutine(ShakeTick(newShake));
            //Update shake
            ShakeUpdate();
        }else{
            Debug.Log("Shake Failed to Apply");
        }
    }

    IEnumerator ShakeTick(shakeInfo shake){
        yield return new WaitForSeconds(shake.shakeDuration);
        shakes.Remove(shake);
        highestShake = 0;
        ShakeUpdate();
    }

    void ShakeUpdate(){
        //Order the list from most severe to least
        shakes.Sort(delegate(shakeInfo a, shakeInfo b)
        {return b.shakeIntensity.CompareTo(a.shakeIntensity);});
        if(shakes.Count > 0){
            if(shakes[0].shakeIntensity > highestShake){
                //If the new shake is stronger
                if(shaker != null){
                    StopCoroutine(shaker);
                    shaker = null;
                }
                highestShake = shakes[0].shakeIntensity;
                shaker = ShakeAction(shakes[0]);
                StartCoroutine(shaker);
            }
        }else{
            StopCoroutine(shaker);
            shaker = null;
        }
    }   

    private IEnumerator ShakeAction(shakeInfo shake)
    {
        while(shake.shakeDuration > 0){
            Feedback();
            target.localPosition = initialPos;
            var randomPoint = new Vector3(target.localPosition.x + (Random.Range(-1f, 1f) * shake.shakeIntensity), target.localPosition.y + (Random.Range(-1f, 1f) * shake.shakeIntensity), initialPos.z);
            target.localPosition = randomPoint;
            for(int i = shakes.Count - 1; i >= 0; i--)
            {
                shakes[i].shakeDuration -= Time.deltaTime;
            }
            yield return null;
        }
        target.localPosition = initialPos;
    } 

    public virtual void Feedback(){}
}

public class shakeInfo{
    public float shakeIntensity = 1f;
    public float shakeDuration = 0f;
}
