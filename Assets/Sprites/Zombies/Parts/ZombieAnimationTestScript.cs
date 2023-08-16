using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombieAnimationTestScript : MonoBehaviour
{

    private List<Animator> _animators;

    private IEnumerator Start()
    {
        _animators = GetComponentsInChildren<Animator>().ToList();

        // idle
        yield return new WaitForSeconds(2f);

        SetBool("alerted", true);

        SetBool("isWalking", true);

        // transition -> walking
        yield return new WaitForSeconds(3f);

        SetBool("isWalking", false);
        SetTrigger("puke");

        // transition -> puking
        yield return new WaitForSeconds(3f);

        SetBool("isPuking", false);
        SetBool("isBiting", true);

        // transition -> biting
        yield return new WaitForSeconds(3f);

        // die
        SetTrigger("die");
    }



    private void SetBool(string boolName, bool value)
    {
        foreach (var animator in _animators) {
            animator.SetBool(boolName, value);
        }
    }

    private void SetTrigger(string triggerName)
    {
        foreach (var animator in _animators) {
            animator.SetTrigger(triggerName);
        }
    }

}
