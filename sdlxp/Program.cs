using System;
using SDL2;

class Program
{
    static IntPtr window;
    static IntPtr renderer;
    static bool quit = false;
    static int x = 0, y = 0;

    static void Main(string[] args)
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");
            return;
        }

        window = SDL.SDL_CreateWindow("SDL2 Test",
                                      SDL.SDL_WINDOWPOS_UNDEFINED,
                                      SDL.SDL_WINDOWPOS_UNDEFINED,
                                      800, 600,
                                      SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (window == IntPtr.Zero)
        {
            Console.WriteLine($"Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
            return;
        }

        renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        if (renderer == IntPtr.Zero)
        {
            Console.WriteLine($"Renderer could not be created! SDL_Error: {SDL.SDL_GetError()}");
            return;
        }

        SDL.SDL_Event e;

        while (!quit)
        {
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    quit = true;
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    HandleKeyPress(e.key.keysym.sym);
                }
            }

            Update();
            Render();
            SDL.SDL_Delay(16); // Cap at roughly 60 fps
        }

        Cleanup();
    }

    static void HandleKeyPress(SDL.SDL_Keycode key)
    {
        switch (key)
        {
            case SDL.SDL_Keycode.SDLK_UP:
                y -= 5;
                break;
            case SDL.SDL_Keycode.SDLK_DOWN:
                y += 5;
                break;
            case SDL.SDL_Keycode.SDLK_LEFT:
                x -= 5;
                break;
            case SDL.SDL_Keycode.SDLK_RIGHT:
                x += 5;
                break;
            case SDL.SDL_Keycode.SDLK_ESCAPE:
                quit = true;
                break;
        }
    }

    static void Update()
    {
        // Add any game logic here
    }

    static void Render()
    {
        SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
        SDL.SDL_RenderClear(renderer);

        // Draw a red rectangle
        SDL.SDL_Rect rect = new SDL.SDL_Rect { x = x, y = y, w = 50, h = 50 };
        SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
        SDL.SDL_RenderFillRect(renderer, ref rect);

        SDL.SDL_RenderPresent(renderer);
    }

    static void Cleanup()
    {
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}
