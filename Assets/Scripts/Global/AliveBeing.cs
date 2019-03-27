using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    public class AliveBeing : MonoBehaviour
    {
        public float acceleration;
        public Vector3 inputAxis;
        public bool controlledByKeyboard = true;
        public float maxSpeed = 10f;
        public float killZ = -10f;
        public float health = 100;
        public string name = "Destrilt";
        public float steering = .7f;

        public static int instancesCount;

        Rigidbody rb;
        // Start is called before the first frame update
        void Start()
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();


            instancesCount++;
            name += "-" + instancesCount;
        }

        // Update is called once per frame
        void Update()
        {
            if (controlledByKeyboard)
                HandleKeyboardInput();
            else
                AICalculateInput();

            UpdateRotation();
        }

        void FixedUpdate()
        {

            /**
            if (inputAxis == Vector3.zero)
                inputAxis = rb.angularVelocity.normalized * -1; //back to original rotation
            rb.AddTorque(inputAxis * steering);
            */
            steering = Mathf.Min(steering, 1f);

            Vector3 force = (inputAxis.normalized * steering + transform.forward * (1f - steering));
            force = transform.forward;
            force *= acceleration;
            if (rb.velocity.y == 0f)
                rb.AddForce(acceleration * force);

            LimitMaxSpeed();
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
            Food nearest = GetNearestFood();
            if (nearest == null)
                return;

            Vector3 inputAxisTmp = (nearest.transform.position - transform.position).normalized;

            inputAxis.x = inputAxisTmp.x;
            inputAxis.z = inputAxisTmp.z;
        }

        void CheckKillZ()
        {
            if (transform.position.z < killZ)
                TakeDamage(health);
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
            Debug.Log(name = " died. R.I.P.");
            GameObject.Destroy(this.gameObject);
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
            float nearestDistance = 0f;
            foreach (Food food in allFoods)
            {
                float dist = DistanceBetween(transform.position, food.transform.position);
                if (nearest == null || dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearest = food;
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
    }

}