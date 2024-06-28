using System;
using System.Collections.Generic;
using SDL2;

class GameObject
{
    public int X, Y;
    public int Width, Height;
    public SDL.SDL_Color Color;

    public GameObject(int x, int y, int width, int height, SDL.SDL_Color color)
    {
        X = x; Y = y; Width = width; Height = height; Color = color;
    }

    public bool IsPointInside(int px, int py)
    {
        return px >= X && px <= X + Width && py >= Y && py <= Y + Height;
    }
}

class Game
{
    private const int WINDOW_WIDTH = 800;
    private const int WINDOW_HEIGHT = 600;
    private const int WORLD_WIDTH = 2000;
    private const int WORLD_HEIGHT = 2000;

    private IntPtr window;
    private IntPtr renderer;
    private bool quit = false;
    private int cameraX = 0, cameraY = 0;
    private List<GameObject> objects = new List<GameObject>();
    private GameObject selectedObject = null;
    private Random random = new Random();
    private bool isDragging = false;

    public void Run()
    {
        Initialize();
        GameLoop();
        Cleanup();
    }

    private void Initialize()
    {
        SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
        window = SDL.SDL_CreateWindow("RTS-Style Game", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, WINDOW_WIDTH, WINDOW_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        // Create some initial objects
        for (int i = 0; i < 10; i++)
        {
            CreateRandomObject();
        }
    }

    private void GameLoop()
    {
        while (!quit)
        {
            HandleInput();
            Update();
            Render();
            SDL.SDL_Delay(16); // Cap at roughly 60 fps
        }
    }

    private void HandleInput()
    {
        SDL.SDL_Event e;
        while (SDL.SDL_PollEvent(out e) != 0)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    quit = true;
                    break;
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    HandleKeyPress(e.key.keysym.sym);
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    HandleMouseClick(e.button);
                    break;
            }
        }
    }

    private void HandleKeyPress(SDL.SDL_Keycode key)
    {
        int moveSpeed = 10;
        switch (key)
        {
            case SDL.SDL_Keycode.SDLK_UP: cameraY = Math.Max(0, cameraY - moveSpeed); break;
            case SDL.SDL_Keycode.SDLK_DOWN: cameraY = Math.Min(WORLD_HEIGHT - WINDOW_HEIGHT, cameraY + moveSpeed); break;
            case SDL.SDL_Keycode.SDLK_LEFT: cameraX = Math.Max(0, cameraX - moveSpeed); break;
            case SDL.SDL_Keycode.SDLK_RIGHT: cameraX = Math.Min(WORLD_WIDTH - WINDOW_WIDTH, cameraX + moveSpeed); break;
            case SDL.SDL_Keycode.SDLK_SPACE: CreateRandomObject(); break;
            case SDL.SDL_Keycode.SDLK_ESCAPE: quit = true; break;
        }
    }

    private void HandleMouseClick(SDL.SDL_MouseButtonEvent button)
    {
        if (button.button == SDL.SDL_BUTTON_LEFT)
        {
            int mouseX = button.x + cameraX;
            int mouseY = button.y + cameraY;

            if (button.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
            {
                selectedObject = objects.Find(obj => obj.IsPointInside(mouseX, mouseY));
                isDragging = selectedObject != null;
            }
            else if (button.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP)
            {
                isDragging = false;
            }
        }
    }

    private void Update()
    {
        if (selectedObject != null && isDragging)
        {
            SDL.SDL_GetMouseState(out int mouseX, out int mouseY);
            selectedObject.X = mouseX + cameraX - selectedObject.Width / 2;
            selectedObject.Y = mouseY + cameraY - selectedObject.Height / 2;

            // Keep object within world bounds
            selectedObject.X = Math.Clamp(selectedObject.X, 0, WORLD_WIDTH - selectedObject.Width);
            selectedObject.Y = Math.Clamp(selectedObject.Y, 0, WORLD_HEIGHT - selectedObject.Height);
        }
    }

    private void Render()
    {
        SDL.SDL_SetRenderDrawColor(renderer, 200, 200, 200, 255);
        SDL.SDL_RenderClear(renderer);

        foreach (var obj in objects)
        {
            SDL.SDL_Rect rect = new SDL.SDL_Rect
            {
                x = obj.X - cameraX,
                y = obj.Y - cameraY,
                w = obj.Width,
                h = obj.Height
            };
            SDL.SDL_SetRenderDrawColor(renderer, obj.Color.r, obj.Color.g, obj.Color.b, obj.Color.a);
            SDL.SDL_RenderFillRect(renderer, ref rect);
        }

        RenderWorldBorder();

        SDL.SDL_RenderPresent(renderer);
    }

    private void RenderWorldBorder()
    {
        SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
        SDL.SDL_Rect borderRect = new SDL.SDL_Rect
        {
            x = -cameraX,
            y = -cameraY,
            w = WORLD_WIDTH,
            h = WORLD_HEIGHT
        };
        SDL.SDL_RenderDrawRect(renderer, ref borderRect);
    }

    private void CreateRandomObject()
    {
        int x = random.Next(0, WORLD_WIDTH - 50);  // Ensure object is fully within world bounds
        int y = random.Next(0, WORLD_HEIGHT - 50);
        int width = random.Next(20, 50);
        int height = random.Next(20, 50);
        SDL.SDL_Color color = new SDL.SDL_Color
        {
            r = (byte)random.Next(256),
            g = (byte)random.Next(256),
            b = (byte)random.Next(256),
            a = 255
        };
        objects.Add(new GameObject(x, y, width, height, color));
    }

    private void Cleanup()
    {
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}

class Program
{
    static void Main(string[] args)
    {
        new Game().Run();
    }
}
