#include <Windows.h>
#include <stdio.h>
#include <stdint.h>
#include <string.h>
#include <time.h>
#include "lvgl.h"
#include "LvglWindowsIconResource\LvglWindowsIconResource.h"
#include "custom_fonts.h"


#define SCREEN_WIDTH 480
#define SCREEN_HEIGHT 272

int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{

    lv_init();

#if LV_TXT_ENC == LV_TXT_ENC_UTF8
    SetConsoleCP(CP_UTF8);
    SetConsoleOutputCP(CP_UTF8);
#endif

    int32_t zoom_level = 100;
    bool allow_dpi_override = true;
    bool simulator_mode = true;
    lv_display_t* display = lv_windows_create_display(
        L"Test Font Converter For LVGL",
        SCREEN_WIDTH,
        SCREEN_HEIGHT,
        zoom_level,
        allow_dpi_override,
        simulator_mode);
    if (!display)
    {
        return -1;
    }

    HWND window_handle = lv_windows_get_display_window_handle(display);
    if (!window_handle)
    {
        return -1;
    }

    HICON icon_handle = LoadIconW(
        GetModuleHandleW(NULL),
        MAKEINTRESOURCE(IDI_LVGL_WINDOWS));
    if (icon_handle)
    {
        SendMessageW(
            window_handle,
            WM_SETICON,
            TRUE,
            (LPARAM)icon_handle);
        SendMessageW(
            window_handle,
            WM_SETICON,
            FALSE,
            (LPARAM)icon_handle);
    }

    lv_indev_t* pointer_indev = lv_windows_acquire_pointer_indev(display);
    if (!pointer_indev)
    {
        return -1;
    }

    lv_indev_t* keypad_indev = lv_windows_acquire_keypad_indev(display);
    if (!keypad_indev)
    {
        return -1;
    }

    lv_indev_t* encoder_indev = lv_windows_acquire_encoder_indev(display);
    if (!encoder_indev)
    {
        return -1;
    }
    lv_task_handler();


    lv_theme_t* theme = lv_theme_default_init(display, lv_palette_main(LV_PALETTE_BLUE), lv_palette_main(LV_PALETTE_RED), false, LV_FONT_DEFAULT);
    lv_display_set_theme(display, theme);

    lv_obj_t* scr_main = lv_obj_create(NULL);

    static int32_t col_dsc[] = { LV_PCT(100), LV_GRID_TEMPLATE_LAST };
    static int32_t row_dsc[] = { LV_PCT(50), LV_PCT(50), LV_GRID_TEMPLATE_LAST };

    lv_obj_t* cont = lv_obj_create(scr_main);
    lv_obj_set_style_grid_column_dsc_array(cont, col_dsc, 0);
    lv_obj_set_style_grid_row_dsc_array(cont, row_dsc, 0);
    lv_obj_set_size(cont, lv_pct(100), lv_pct(100));
    lv_obj_center(cont);
    lv_obj_set_layout(cont, LV_LAYOUT_GRID);

    lv_obj_t* TextArea1 = lv_textarea_create(cont);
    lv_obj_set_width(TextArea1, lv_pct(80));
    lv_obj_set_height(TextArea1, lv_pct(80));
    lv_obj_set_align(TextArea1, LV_ALIGN_CENTER);
    lv_obj_set_style_text_font(TextArea1, &badr_25, 0);
    //lv_textarea_set_placeholder_text(TextArea1, "متن را وارد نمایید");
    lv_obj_set_grid_cell(TextArea1, LV_GRID_ALIGN_STRETCH, 0, 1,
        LV_GRID_ALIGN_STRETCH, 0, 1);

    lv_obj_t* TextArea2 = lv_textarea_create(cont);
    lv_obj_set_width(TextArea2, lv_pct(80));
    lv_obj_set_height(TextArea2, lv_pct(80));
    lv_obj_set_align(TextArea2, LV_ALIGN_CENTER);
    lv_obj_set_style_text_font(TextArea2, &merienda_25, 0);
    //lv_textarea_set_placeholder_text(TextArea2, "Enter text here");
    lv_obj_set_grid_cell(TextArea2, LV_GRID_ALIGN_STRETCH, 0, 1,
        LV_GRID_ALIGN_STRETCH, 1, 1);

    lv_screen_load(scr_main);


    while (1)
    {
        lv_task_handler();
        lv_delay_ms(5);
    }

    return 0;
}
