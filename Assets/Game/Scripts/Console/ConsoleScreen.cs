using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Game.Scripts.Console.Graphics.Shapes;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Graphics = Game.Scripts.Console.Graphics.Graphics;

namespace Game.Console
{
    
    public class ConsoleScreen : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f, rotationSpeed = 10f;
        [SerializeField] private TextMeshProUGUI screenTMP;
        private Graphics _consoleGraphics;

        private void Start()
        {
            var fdp = screenTMP.fontSize / screenTMP.font.faceInfo.pointSize;
            var charWidth = screenTMP.font.characterTable['A'].glyph.metrics.horizontalAdvance * fdp;
            var charHeight = screenTMP.font.faceInfo.lineHeight * fdp;
            var charAspect = (float)charWidth / charHeight;
            var textWidth = Mathf.FloorToInt(screenTMP.rectTransform.rect.width / charWidth);
            var textHeight = Mathf.FloorToInt(screenTMP.rectTransform.rect.height / charHeight);
            
            Debug.Log(charWidth);
            Debug.Log(charHeight);
            Debug.Log(charAspect);
            Debug.Log(textWidth);
            Debug.Log(textHeight);
            
            _consoleGraphics = new Graphics(charAspect, textWidth, textHeight);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }

        private void Update()
        {
            HandleInput();
            // _consoleGraphics.LightPos = _consoleGraphics.Camera.position;
            _consoleGraphics.Update();

            if (_consoleGraphics.Shapes[0] is Sphere sphere)
            {
                // sphere.Center = new Vector3(Mathf.Cos(Time.time), -0.1f, Mathf.Sin(Time.time)) * 5f;
            }
            
            
            screenTMP.text = _consoleGraphics.GetScreen();
        }

        private void HandleInput()
        {
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            var rotSpeed = rotationSpeed * Time.deltaTime;
            _consoleGraphics.Camera.pitch -= mouseY * rotSpeed;
            _consoleGraphics.Camera.yaw += mouseX * rotSpeed;
            
            
            _consoleGraphics.Camera.pitch = Mathf.Clamp(_consoleGraphics.Camera.pitch, -89, 89);

            float h = 0, v = 0;
            if (Input.GetKey(KeyCode.W))
                v = 1;
            if (Input.GetKey(KeyCode.S))
                v = -1;
            if (Input.GetKey(KeyCode.A))
                h = -1;
            if (Input.GetKey(KeyCode.D))
                h = 1;

            var forward = _consoleGraphics.Camera.Forward;
            var right = _consoleGraphics.Camera.Right;
            var movement = forward * v + right * h;
            _consoleGraphics.Camera.position += (moveSpeed * Time.deltaTime) * movement;

            if (Input.GetKeyDown(KeyCode.LeftShift)) _consoleGraphics.ReflectLimit--;
            if (Input.GetKeyDown(KeyCode.Tab)) _consoleGraphics.ReflectLimit++;
            
            // if (Input.GetMouseButtonDown(0)) _consoleGraphics.RayDestroy();
        }
    }

}