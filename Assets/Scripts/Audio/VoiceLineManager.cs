using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineManager : MonoBehaviour
{
    [System.Serializable]
    public class VoiceLine
    {
        public AK.Wwise.Event line;
        [HideInInspector] public bool onCooldown = false;
        public float cooldownTime = 3f;
    }

    [SerializeField] private float fastThreshold = 5f;

    [SerializeField] private AK.Wwise.Event characterSwitch;
    [SerializeField] private VoiceLine goingFast;
    [SerializeField] private VoiceLine grindExit;
    [SerializeField] private VoiceLine boxHit;
    [SerializeField] private VoiceLine raceFinish;
    [SerializeField] private VoiceLine noBoost;

    private Rigidbody2D playerRB;

    // Start is called before the first frame update
    void Start()
    {
        characterSwitch.Post(gameObject);
        playerRB = GetComponent<Rigidbody2D>();
        BSPlayerController controller = GetComponent<BSPlayerController>();
        controller.onOverheatedBlast.AddListener(() => { PlayLine(noBoost); });
        controller.onGoodGrindEnd.AddListener(() => { PlayLine(grindExit); });
        controller.onSlowed.AddListener(() => { PlayLine(boxHit); });
        controller.onWin.AddListener(() => { PlayLine(raceFinish, true); Debug.Log("Win"); });
    }

    private void Update()
    {
        FastLineCheck();
    }

    private void FastLineCheck()
    {
        if (!goingFast.onCooldown && playerRB.velocity.magnitude > fastThreshold)
        {
            PlayLine(goingFast);
        }
    }

    private IEnumerator Cooldown(VoiceLine line)
    {
        line.onCooldown = true;
        yield return new WaitForSeconds(line.cooldownTime);
        line.onCooldown = false;
    }

    public void PlayLine(VoiceLine line, bool ignoreCoolDown = false)
    {
        if (ignoreCoolDown || !line.onCooldown)
        {
            line.line.Post(gameObject);
            StartCoroutine(Cooldown(line));
        }
    }
}
