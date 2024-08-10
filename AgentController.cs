using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class AgentController : Agent
{

    //Pellet variables below
    [SerializeField] private Transform target;
    public int pelletCount;
    public GameObject food; 
    [SerializeField] private List<GameObject> spawnedPelletsList= new List<GameObject>();

    //Agent variables below
    [SerializeField] private float moveSpeed =4f;
    private Rigidbody rb;

    //Environment variables
    [SerializeField] private Transform environmentLocation;



    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        //Agent Random spawn
        transform.localPosition = new Vector3(Random.Range(-4,4f),0.3f,Random.Range(-4f,4f));
        //pellet spawn random
        CreatePellet();
        //target.localPosition = new Vector3(Random.Range(-4,4f),0.3f,Random.Range(-4f,4f));
    }
    public void CreatePellet()
    {   

        if(spawnedPelletsList.Count !=0)
        {
            RemovePellet(spawnedPelletsList);
        }
        for(int i=0;i<pelletCount;i++)
        {
            GameObject newPellet =Instantiate(food);
            newPellet.transform.parent = environmentLocation;
            Vector3 pelletLocation =new Vector3(Random.Range(-4,4f),0.3f,Random.Range(-4f,4f));
            newPellet.transform.localPosition = pelletLocation;
            spawnedPelletsList.Add(newPellet);
            Debug.Log("Spawned");
        }
    }
    private void RemovePellet(List<GameObject>toBeDeletedGameObjectList)
    {
        foreach(GameObject i in toBeDeletedGameObjectList)
        {
            Destroy(i.gameObject);
        
        }
        toBeDeletedGameObjectList.Clear();    
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
       // sensor.AddObservation(target.localPosition);
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate =actions.ContinuousActions[0];
        float moveForward =actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward *moveForward*moveSpeed*Time.deltaTime);
        transform.Rotate(0f,moveRotate * moveSpeed,0f,Space.Self);

        
        /*
        Vector3 velocity = new Vector3(moveX,0f,moveZ);
        velocity = velocity.normalized * Time.deltaTime*moveSpeed;
        transform.localPosition += velocity;
        */
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float>continuousActions =actionsOut.ContinuousActions;
        continuousActions[0]=Input.GetAxisRaw("Horizontal");
        continuousActions[1]=Input.GetAxisRaw("Vertical");
    }   
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Pellet")
        {
            spawnedPelletsList.Remove(other.gameObject);
            Destroy(other.gameObject);
            AddReward(10f);
            if(spawnedPelletsList.Count ==0)
            {   
                RemovePellet(spawnedPelletsList);
                AddReward(5f);
                EndEpisode();
            }
        }
        if(other.gameObject.tag=="Wall")
        {   
            RemovePellet(spawnedPelletsList);
            AddReward(-1f);
            EndEpisode();
        }
    }   
}