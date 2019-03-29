using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    public class AliveBeing : MonoBehaviour
    {
        public int maxEntities = 50;
        public float acceleration;
        public Vector3 inputAxis;
        public bool controlledByKeyboard = true;
        public float maxSpeed = 10f;
        public float health = 100;
        public string name = "Destrilt";
        public float steering = .7f;

        public float minRadiusFood = 1f;
        public float maxRadiusFood = 10f;
        public float minRadiusPoison = 1f;
        public float maxRadiusPoison = 10f;

        public float visionEntropy = .1f;
        public float intelligenceEntropy = .1f;

        public Vector3 worldBounds = new Vector3(15f, 15f, 15f);

        public float healthDecrementBySeconds = 3f;

        public static int instancesCount;

        public float debugLineSize = 3f;

        public float[] dna = { 0f, 0f, 0f, 0f };
        public float fertilityRate = .01f;
        public GameObject FoodTemplate;

        bool randomizeDNA = true;

        public AliveBeing parent = null;


        Rigidbody rb;

        [SerializeField] float steeringFood;
        [SerializeField] float steeringPoison;
        [SerializeField] Animator animator;

        public static int aliveCount;
        bool flipFlop; 

        public static string randomName
        {
            get
            {
                return names[Random.Range(0, names.Count - 1)];
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();


            instancesCount++;
            name = randomName + "-" + instancesCount;
            if(randomizeDNA)
                RandomizeDNA();

            aliveCount++;
            gameObject.name = name;
        }

        // Update is called once per frame
        void Update()
        {
            if (controlledByKeyboard)
                HandleKeyboardInput();
            else
                AICalculateInput();

            UpdateRotation();

            DrawLines();


            TakeDamage(healthDecrementBySeconds * Time.deltaTime);

            if (Random.value <= fertilityRate && aliveCount < maxEntities)
                Clone();

            ConnectToParent();

            float speed = rb.velocity.magnitude / maxSpeed;
            speed = Mathf.Min(speed, 1f);
            if (animator)
                animator.SetFloat("SpeedNormalized", speed);


        }
        void OnDrawGizmos()
        {
            if (flipFlop)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, this.dna[2]);

            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, this.dna[3]);
            }


            flipFlop = !flipFlop;
        }
        void ConnectToParent()
        {
            if (!parent)
                return;

            Vector3 lineEnd = parent.transform.position;

            Debug.DrawLine(transform.position, lineEnd, Color.cyan);


        }
        public void Mutate(float[] dna, float fertilityRate)
        {
            dna[0] += Random.Range(Mathf.Abs(intelligenceEntropy) * -1, Mathf.Abs(intelligenceEntropy));
            dna[1] += Random.Range(Mathf.Abs(intelligenceEntropy) * -1, Mathf.Abs(intelligenceEntropy));
            dna[2] += Random.Range(Mathf.Abs(visionEntropy) * -1, Mathf.Abs(visionEntropy));
            dna[3] += Random.Range(Mathf.Abs(visionEntropy) * -1, Mathf.Abs(visionEntropy));

            fertilityRate += Random.Range(Mathf.Abs(fertilityRate) * -1, Mathf.Abs(fertilityRate));
            this.fertilityRate = fertilityRate;

            this.dna = dna;
        }

        public void Clone()
        {
            AliveBeing baby = ObjectPool.GetFromPool<AliveBeing>(this.gameObject);
            baby.transform.position = transform.position;
            baby.transform.rotation = transform.rotation;
            baby.Awake();
            baby.Mutate(dna, fertilityRate);

            Debug.Log("A new baby is alive: " + baby.name);

            baby.parent = this;
            baby.randomizeDNA = false;
        }
        void DrawLines()
        {
            Vector3 lineEndFood = (debugLineSize * CalcSteeringFood()) + transform.position;
            Vector3 lineEndPoison = (debugLineSize * CalcSteeringPoison()) + transform.position;
            lineEndFood = Vector3.Lerp(transform.position, lineEndFood, Mathf.Abs(this.dna[0]));
            lineEndPoison = Vector3.Lerp(transform.position, lineEndPoison, Mathf.Abs(this.dna[1]));

            Debug.DrawLine(transform.position, lineEndFood, Color.green);
            Debug.DrawLine(transform.position, lineEndPoison, Color.red);
        }
        void FixedUpdate()
        {

            /**
            if (inputAxis == Vector3.zero)
                inputAxis = rb.angularVelocity.normalized * -1; //back to original rotation
            rb.AddTorque(inputAxis * steering);
            */
            steering = Mathf.Min(steering, 1f);
            LimitMaxSpeed();

            
            Vector3 force = inputAxis * acceleration;

            rb.AddForce(acceleration * force);

        }
        void RandomizeDNA()
        {
            this.dna[0] = Random.Range(-1f, 1f);
            this.dna[1] = Random.Range(-1f, 1f);
            this.dna[2] = Random.Range(minRadiusFood, maxRadiusFood);
            this.dna[3] = Random.Range(minRadiusPoison, maxRadiusPoison);
        }
        Vector3 CalcSteeringFood()
        {
            Food nearest = GetNearestFood();
            if (nearest == null)
                return Vector3.zero;

            return (nearest.transform.position - transform.position).normalized;
        }
        Vector3 CalcSteeringPoison()
        {
            Poison nearest = GetNearestPoison();
            if (nearest == null)
                return Vector3.zero;

            return (nearest.transform.position - transform.position).normalized;
        }
        void LimitMaxSpeed()
        {
            if (rb.velocity.magnitude > maxSpeed)
                rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        void HandleKeyboardInput()
        {
            inputAxis = Vector2.zero;

            if (Input.GetKey(KeyCode.A))
                inputAxis.y += -1;
            if (Input.GetKey(KeyCode.D))
                inputAxis.y += 1;
            if (Input.GetKey(KeyCode.W))
                inputAxis.x = 1;
            if (Input.GetKey(KeyCode.S))
                inputAxis.x = -1;
        }

        void AICalculateInput()
        {
            if (IsOutOfBounds())
            {
                BackToCenter();
                return;
            }
            Vector3 nearestFoodInput = InSight(GetNearestFood(), this.dna[2]) ? CalcSteeringFood() : Vector3.zero;
            Vector3 nearestPoisonInput = InSight(GetNearestPoison(), this.dna[3]) ? CalcSteeringPoison() : Vector3.zero;

            Vector3 weightedDirection = (this.dna[0] * nearestFoodInput) + (this.dna[1] * nearestPoisonInput);
            weightedDirection = weightedDirection.normalized;

            if (weightedDirection == Vector3.zero)
                weightedDirection = transform.forward;

            inputAxis = weightedDirection;
        }
        bool IsOutOfBounds()
        {
            return !IsInBounds();
        }
        bool IsInBounds()
        {
            Vector3 p = transform.position;
            Vector3 b = worldBounds;
            return (
               Mathf.Abs(p.x) < worldBounds.x &&
               Mathf.Abs(p.y) < worldBounds.y &&
               Mathf.Abs(p.z) < worldBounds.z
            );
        }
        void BackToCenter()
        {
            inputAxis = transform.position.normalized * - 1; //back to Vector3.zero
        }
        bool InSight(Collectable collectable, float visionRadius)
        {
            if (!collectable)
                return false;

            return DistanceBetween(collectable.transform.position, transform.position) <= visionRadius;
        }


        public void TakeDamage(float amount)
        {
            health -= amount;
            NormalizeHealth();
        }
        public void GainHealth(float amount)
        {
            health += amount;
            NormalizeHealth();
        }
        void NormalizeHealth()
        {
            ClampHealth();
            CheckDeath();
        }
        void CheckDeath()
        {
            if (health == 0f)
                Die();
        }
        void Die()
        {
            Debug.Log(name + " died.");
            if (FoodTemplate != null)
            {
                Collectable food = ObjectPool.GetFromPool<Collectable>(FoodTemplate);
                food.transform.position = transform.position;
            }

            aliveCount--;

            ObjectPool.AddToPool<AliveBeing>(this);
        }
        void ClampHealth()
        {
            health = Mathf.Clamp(health, 0, 100f);
        }
        void OnTriggerEnter(Collider other)
        {
            Collectable collectable = other.GetComponent<Collectable>();
            if (collectable)
            {
                GainHealth(collectable.healthIncrement);
                collectable.Consume();
            }
        }

        Food GetNearestFood()
        {
            List<Food> allFoods = Food.GetAllFoods();
            Food nearest = null;
            float nearestDistance = float.PositiveInfinity;
            foreach (Food food in allFoods)
            {
                float dist = DistanceBetween(transform.position, food.transform.position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearest = food;
                }
            }
            return nearest;
        }
        Poison GetNearestPoison()
        {
            List<Poison> allPoisons = Poison.GetAllPoisons();
            Poison nearest = null;
            float nearestDistance = float.PositiveInfinity;
            foreach (Poison poison in allPoisons)
            {
                float dist = DistanceBetween(transform.position, poison.transform.position);
                if(dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearest = poison;
                }
            }
            return nearest;
        }
        public static float DistanceBetween(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude;
        }

        public static Quaternion DoSlerp(Vector3 rotationA, Vector3 rotationB, float alpha = .5f)
        {
            return DoSlerp(Quaternion.Euler(rotationA), Quaternion.Euler(rotationB), alpha);
        }

        public static Quaternion DoSlerp(Quaternion rotationA, Quaternion rotationB, float alpha = .5f)
        {
            return Quaternion.Slerp(rotationA, rotationB, alpha);

        }
        void UpdateRotation()
        {

            Quaternion desired = Quaternion.LookRotation(inputAxis);
            Quaternion current = transform.rotation;

            transform.rotation = DoSlerp(current, desired, steering);
        }
        static List<string> names = new List<string>(new string[]{ "Michael", "Logan", "Carter", "Avery", "Jayden", "Madison", "Ryan", "Riley", "Julian", "Hunter", "Cameron", "Jordan", "Addison", "Adrian", "Parker", "Evan", "Carson", "Tyler", "Taylor", "Micah", "Maxwell", "Reagan", "Ashton", "Kai", "Ashley", "Alex", "Bailey", "Jade", "Morgan", "Jude" });

    };

    

}