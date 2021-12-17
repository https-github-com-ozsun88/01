#pragma once

class AlwaysOnTop
{
public:
    AlwaysOnTop();
    ~AlwaysOnTop();

    void Init();

protected:
    static LRESULT CALLBACK WndProc_Helper(HWND window, UINT message, WPARAM wparam, LPARAM lparam) noexcept
    {
        auto thisRef = reinterpret_cast<AlwaysOnTop*>(GetWindowLongPtr(window, GWLP_USERDATA));

        if (!thisRef && (message == WM_CREATE))
        {
            const auto createStruct = reinterpret_cast<LPCREATESTRUCT>(lparam);
            thisRef = reinterpret_cast<AlwaysOnTop*>(createStruct->lpCreateParams);
            SetWindowLongPtr(window, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(thisRef));
        }

        return thisRef ? thisRef->WndProc(window, message, wparam, lparam) :
            DefWindowProc(window, message, wparam, lparam);
    }

private:
    static inline AlwaysOnTop* s_instance = nullptr;

    HWND hotKeyHandleWindow{ nullptr };
    std::vector<HWND> topmostWindows;

    LRESULT WndProc(HWND, UINT, WPARAM, LPARAM) noexcept;

    void ProcessCommand(HWND window);
    void ResetAll();
    void CleanUp();

    bool IsTopmost(HWND window) const noexcept;
    bool SetTopmostWindow(HWND window) const noexcept;
    bool ResetTopmostWindow(HWND window) const noexcept;
};

