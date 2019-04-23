using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public bool PlayerSpell;
    private Player player;
    private bool ChargeComplete = false;
    public GameObject OwnerGO;
    private GameObject Master;
    public GameObject EndPoint;
    public Combat ThisSpell;
    public GameObject NearestHostile;
    public List<GameObject> HostileList;
    private NPC npc;
    private bool newSwarm = false;
    private Swarm swarm_left;
    private Swarm swarm_right;

    private class Swarm
    {
        public bool invertPath;
        private bool atDestination = false;
        public List<GameObject> bugs;
        public List<float> Deviances;
        private float MaxD;
        private Vector3 StartDest;
        private Vector3 EndDest;
        private float StartBound;
        private float EndBound;
        private float CurrentPosition;
        private float speed;

        public Swarm(List<GameObject> bugs, float StartBound, float EndBound, Vector3 StartDest, Vector3 EndDest, float speed, bool invert = false)
        {
            this.speed = speed;
            invertPath = invert;
            this.StartDest = StartDest;
            this.EndDest = EndDest;
            this.StartBound = StartBound;
            this.EndBound = EndBound;
            atDestination = false;
            CurrentPosition = StartBound;
            Deviances = new List<float>(0);
            for (int x = 0; x < bugs.Count; x++)
            {
                //float offset = Random.Range(1, 2);
                Deviances.Add(0.0f);
            }
            this.bugs = bugs;
        }

        public bool AtDestination()
        {
            return atDestination;
        }

        public void UpdatePos()
        {
            CurrentPosition += speed;
            if (CurrentPosition > EndBound)
            {
                CurrentPosition = EndBound;
            }
            float distFactor = (EndBound - StartBound) / Vector3.Distance(StartDest, EndDest);
            Quaternion baseRotation = Quaternion.Euler(90, 0, 0);
            Quaternion DesiredRotation = Quaternion.FromToRotation(StartDest, EndDest);
            float dir = 1.0f;
            if (invertPath)
            {
                dir = -1.0f;
            }

            for (int i = 0; i < bugs.Count; i++)
            {
                //TODO determine deviances[i]

                //sin(a(z+pi/4))+d
                Vector3 pos = bugs[i].transform.position;
                float deltaX = dir * Mathf.Sin(distFactor * (CurrentPosition)) + Deviances[i];
                float deltaZ = /*distFactor **/ (CurrentPosition - StartBound) / distFactor;
                pos.x += deltaX;
                pos.z += deltaZ;
                //apply rotation

                //
                bugs[i].transform.position = pos;
            }
            if (CurrentPosition == EndBound)
            {
                atDestination = true;
                DestroyBugs();
            }
        }

        private void DestroyBugs()
        {
            foreach (GameObject bug in bugs)
            {
                //GameObject.Destroy(bug);
            }
        }

    }


    // Use this for initialization
    void Start()
    {
        Master = GameObject.FindGameObjectWithTag("Master");

        //clearly not definitive
        if (Vector3.Distance(transform.position, Master.transform.position) > 0.5f)
        {
            PlayerSpell = false;
            npc = OwnerGO.GetComponent<NPC>();
        }
        else
        {
            PlayerSpell = true;
            player = OwnerGO.GetComponent<Player>();
        }

        if (ThisSpell.Spellelement == Combat.SpellElements.Lightning)
        {
            //sets hostile as lightning end point if hostile is within range
            if (PlayerSpell == true)
                HostileList = player.HostileList;
            else
                HostileList = npc.HostileList;

            float distance = Mathf.Infinity;
            for (int x = 0; x < HostileList.Count; x++)
            {
                Vector3 Diff = HostileList[x].transform.position - EndPoint.transform.position;
                float CurrentDistance = Diff.sqrMagnitude;
                if (CurrentDistance < distance)
                {
                    NearestHostile = HostileList[x];
                    distance = CurrentDistance;
                }
            }
            if (distance < 20.0f)
            {
                DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript Lightning = this.gameObject.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript>();
                Lightning.Destination = NearestHostile;
            }
            //NearestHostile = nearestHostile;
        }
        if (ThisSpell.Spellelement == Combat.SpellElements.Nature)
        {
            //sets hostile as lightning end point if hostile is within range
            if (PlayerSpell == true)
                HostileList = player.HostileList;
            else
                HostileList = npc.HostileList;

            float distance = Mathf.Infinity;
            for (int x = 0; x < HostileList.Count; x++)
            {
                Vector3 Diff = HostileList[x].transform.position - transform.position;
                float CurrentDistance = Diff.sqrMagnitude;
                if (CurrentDistance < distance)
                {
                    NearestHostile = HostileList[x];
                    distance = CurrentDistance;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ThisSpell.AttackName == "")
            return;
        switch (ThisSpell.Spelleffect)
        {
            case Combat.SpellEffect.Projectile:
                //basic spell behavior
                if (ThisSpell.Chargeable == true)
                {
                    if (Input.GetMouseButtonUp(1))
                    {
                        ChargeComplete = true;
                    }
                    if (ChargeComplete == true)
                        transform.Translate(Vector3.forward * Time.deltaTime * 8);
                }
                else
                    transform.Translate(Vector3.forward * Time.deltaTime * 8);
                break;
            case Combat.SpellEffect.Chain:
                //Not Chargable at least for the moment
                this.transform.parent = Master.transform;
                this.transform.rotation = Master.transform.rotation;

                Vector3 Diff = NearestHostile.transform.position - EndPoint.transform.position;
                float Distance = Diff.sqrMagnitude;
                if (Distance > 30.0f)
                {
                    DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript Lightning = this.gameObject.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript>();
                    Lightning.Destination = EndPoint;
                }
                else
                {
                    DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript Lightning = this.gameObject.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript>();
                    Lightning.Destination = NearestHostile;
                }
                if (Input.GetMouseButtonUp(1))
                    Destroy(gameObject);
                break;
            case Combat.SpellEffect.Swarm:
                //Not Chargable at least for the moment
                if (!newSwarm)
                {
                    int numBugs = 6;
                    List<GameObject> bugs_left = new List<GameObject>();
                    List<GameObject> bugs_right = new List<GameObject>();
                    float speed = 0.0001f;
                    float startBound = 7 * Mathf.PI / 16;
                    float endBound = 9 * Mathf.PI / 16;
                    for (int count = 0; count < numBugs; count++)
                    {
                        GameObject lSpell = Instantiate(ThisSpell.Spell, Master.transform.position, transform.rotation);
                        GameObject rSpell = Instantiate(ThisSpell.Spell, Master.transform.position, transform.rotation);
                        lSpell.transform.parent = this.transform;
                        rSpell.transform.parent = this.transform;
                        bugs_left.Add(lSpell);
                        bugs_right.Add(rSpell);
                    }

                    swarm_left = new Swarm(bugs_left, startBound, endBound, Master.transform.position, NearestHostile.transform.position, speed, true);
                    swarm_right = new Swarm(bugs_right, startBound, endBound, Master.transform.position, NearestHostile.transform.position, speed);

                    Debug.Log("swarm target " + NearestHostile.transform.position.ToString());
                    newSwarm = true;
                }
                if (!swarm_left.AtDestination()) //for now assume both swarms reach destination at same time
                {
                    swarm_left.UpdatePos();
                    swarm_right.UpdatePos();
                }
                else
                    Destroy(gameObject);
                break;
            case Combat.SpellEffect.GroundSpikes:
                break;

            }
       // }
    }
    //else
    //{
    //    switch (ThisSpell.Spelleffect)
    //    {
    //        //case Combat.SpellEffect.Projectile:
    //        //    //basic spell behavior
    //        //    transform.Translate(Vector3.forward * Time.deltaTime * 8);
    //        //    break;
    //        //case Combat.SpellEffect.Chain:
    //        //    this.transform.parent = Master.transform;
    //        //    this.transform.rotation = Master.transform.rotation;

    //        //    Vector3 Diff = NearestHostile.transform.position - EndPoint.transform.position;
    //        //    float Distance = Diff.sqrMagnitude;
    //        //    if (Distance > 30.0f)
    //        //    {
    //        //        DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript Lightning = this.gameObject.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript>();
    //        //        Lightning.Destination = EndPoint;
    //        //    }
    //        //    else
    //        //    {
    //        //        DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript Lightning = this.gameObject.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript>();
    //        //        Lightning.Destination = NearestHostile;
    //        //    }
    //        //    if (Input.GetMouseButtonUp(1))
    //        //        Destroy(gameObject);
    //        //    break;
    //        case Combat.SpellEffect.Swarm:
    //            if (!newSwarm)
    //            {
    //                int numBugs = 10;
    //                List<GameObject> bugs_left = new List<GameObject>();
    //                List<GameObject> bugs_right = new List<GameObject>();
    //                float speed = 0.0001f;
    //                float startBound = 7 * Mathf.PI / 16;
    //                float endBound = 9 * Mathf.PI / 16;
    //                for (int count = 0; count < numBugs; count++)
    //                {
    //                    GameObject lSpell = Instantiate(ThisSpell.Spell, Master.transform.position, transform.rotation);
    //                    GameObject rSpell = Instantiate(ThisSpell.Spell, Master.transform.position, transform.rotation);
    //                    bugs_left.Add(lSpell);
    //                    bugs_right.Add(rSpell);
    //                }

    //                swarm_left = new Swarm(bugs_left, startBound, endBound, Master.transform.position, NearestHostile.transform.position, speed, true);
    //                swarm_right = new Swarm(bugs_right, startBound, endBound, Master.transform.position, NearestHostile.transform.position, speed);

    //                Debug.Log("swarm target " + NearestHostile.transform.position.ToString());
    //                newSwarm = true;
    //            }
    //            if (!swarm_left.AtDestination()) //for now assume both swarms reach destination at same time
    //            {
    //                swarm_left.UpdatePos();
    //                swarm_right.UpdatePos();
    //            }
    //            else
    //                Destroy(gameObject);
    //            break;
    //        case Combat.SpellEffect.GroundSpikes:
    //            break;

    //    }
    //}
    //}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Eyes")
            return;
        Debug.Log(col.gameObject.name);
        if (PlayerSpell == true && col.gameObject.tag != "Player")
        {
            Destroy(gameObject);
            //spawn explosion partical system
        }
    }
}
