using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading;

public class Agent
{

    public float[][] value_table;   // The matrix containing the values estimates.
    readonly float learning_rate = 0.005f;   // The rate at which to update the value estimates given a reward.
   


    public Agent(int stateSize, int actionSize, bool optimistic)
    {
        value_table = new float[stateSize][];
        for (int i = 0; i < stateSize; i++)
        {
            value_table[i] = new float[actionSize];
            for (int j = 0; j < actionSize; j++)
            {
                if (!optimistic)
                {
                    value_table[i][j] = 0.0f;
                }
                else
                {
                    value_table[i][j] = 1.0f;
                }
            }
        }
       
    }

    public int PickAction(int state)
    {
        float confidence = 1.9f;
        float[] probs = softmax(value_table[state], confidence);
        float cumulative = 0.0f;
        int selectedElement = 0;
        float diceRoll = Random.Range(0f, 1f);
        for (int i = 0; i < probs.Length; i++)
        {
            cumulative += probs[i];
            if (diceRoll < cumulative)
            {
                selectedElement = i;
                break;
            }
        }
        return selectedElement;
    }


    public void UpdatePolicy(int state, int action, float reward)
    {
        value_table[state][action] += learning_rate * (reward - value_table[state][action]);
    }


    float[] softmax(float[] values, float temp)
    {
        float[] softmax_values = new float[values.Length];
        float[] exp_values = new float[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            exp_values[i] = Mathf.Exp(values[i] / temp);
        }

        for (int i = 0; i < values.Length; i++)
        {
            softmax_values[i] = exp_values[i] / exp_values.Sum();
        }
        return softmax_values;
    }
}



public class Reward : MonoBehaviour
{

    public float totalRewards; // Total rewards obtained over the course of all trials.
    public int trial = 1; // Trial index.
    int num_arms; // Number of chests in a given trial.
    int totalStates; // Number of possible rooms with unique chest reward probabilties. 
    readonly int state = 0; // Index of current room.
    public float actSpeed; // Speed at which actions are chosen.
    float[][] armProbs; // True probability values for each chest in each room.
    Agent agent; // The agent which learns to pick actions.

    public Text at, rt;

    public GameObject d1;
    public GameObject d2;
    public GameObject d3;
    public GameObject d4;

    public GameObject s1;
    public GameObject s2;
    public GameObject s3;
    public GameObject s4;

    public Animator ar;

    public void BeginLearning()
    {
        trial = 0;
        totalRewards = 0;

        //int stateMode = GameObject.Find("state_D").GetComponent<Dropdown>().value;
        actSpeed = 0.2f;
        totalStates = 1;


        num_arms = 4;
        bool optimistic = true;

        agent = new Agent(totalStates, num_arms, optimistic);

        int diff = 1;
        float adjust = ((float)diff) * 0.1f;
        armProbs = new float[totalStates][];
        for (int i = 0; i < totalStates; i++)
        {
            armProbs[i] = new float[num_arms];
            int winner = Random.Range(0, num_arms);
            for (int j = 0; j < num_arms; j++)
            {
                if (j == winner)
                {
                    armProbs[i][j] = Random.Range(0.6f, 1.0f - adjust);
                }
                else
                {
                    armProbs[i][j] = Random.Range(0.0f + adjust, 0.4f);
                }
            }
        }

        //GameObject[] startUI = (GameObject.FindGameObjectsWithTag("loading"));
        //foreach (GameObject obj in startUI)
        //{
        //    Destroy(obj);
        //}

        //chests = new List<GameObject>();
        //estimatedValues = new List<GameObject>();
        //trueValues = new List<GameObject>();

        //GameObject.Find("restartButton").GetComponent<Button>().interactable = true;
        LoadTrial();
    }


    // Start is called before the first frame update
    void Start()
    {
        BeginLearning();
        ar = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown("space"))
        { 
        StartCoroutine(Act());

        }
    }

    public void LoadTrial()
    {
        print("aish");
    }
    IEnumerator Act()
    {



            for (int j = 0; j < 50; j++)
            {


                actSpeed = 2.0f;
                yield return new WaitForSeconds(actSpeed);

                int action = agent.PickAction(state);
                print($"Action {action}");
                print(agent.value_table);

                float[] c = { -1.0f, 1.0f };
                int i = Random.Range(0, 2);
                float reward = c[i];


                print($"Round reward {reward}");
                totalRewards += reward;
                print($"Total reward {totalRewards}");

                agent.UpdatePolicy(state, action, reward);

                if (action == 0)
                {
                    ar.Play("stance");
                }

                if (action == 1)
                {
                    ar.Play("box");
                }

                if (action == 2)
                {
                    ar.Play("step");
                }

                if (action == 3)
                {
                    ar.Play("win");
                }
                //at.text = $"Action {action}";
                //rt.text = $"Round reward {reward}";
                //Thread.Sleep(2000);
            }


        }

}
