#include <Windows.h>
#include <stdio.h>
#include <stdint.h>
#include <string.h>
#include <time.h>
#include <math.h>
#include "lvgl.h"
#include "LvglWindowsIconResource\LvglWindowsIconResource.h"
#include "fonts/custom_fonts.h"
#include "symbols/fontawesome_24_symbol.h"
#include "symbols/material_icons_24_symbol.h"
#include "symbols/noto_emoji_24_symbol.h"
#include "svg/bootstrap_icons_svg.h"

#define SCREEN_WIDTH 900
#define SCREEN_HEIGHT 600

#define TAB1_TITLE "C"
#define TAB2_TITLE "Symbol"
#define TAB3_TITLE "Binary"
#define TAB4_TITLE "SVG"

#define CANVAS_WIDTH  200
#define CANVAS_HEIGHT 200


typedef struct {
    char font_name[255];
    lv_font_t* font;
    char** text;
}fonts_t;

typedef struct {
    char font_name[255];
    lv_font_t* font;
    int symbols_count;
    symbol_def_t* symbols;
}symbols_t;

typedef struct {
    char font_name[255];
    char* path;
    char** text;
}bin_t;

typedef struct {
    char font_name[255];
    int svgs_count;
    int max_svg_buffer;
    svg_def_t* svgs;
}svgs_t;

const char* persian_sample_text =
u8"کتابخانه LVGL یک مجموعه متن‌باز و سبک‌وزن برای توسعه رابط‌های کاربری گرافیکی پیشرفته در سیستم‌های تعبیه‌شده است. این کتابخانه با مدیریت مؤثر حافظه، پشتیبانی از لایه‌ها، انیمیشن‌ها و کنترل‌های قابل سفارشی‌سازی، کارایی و پاسخگویی بالا را فراهم می‌کند.\n";

const char* english_sample_text =
"LVGL is an open-source, lightweight library designed to develop advanced graphical user interfaces on embedded systems. It efficiently manages memory, supports layering, animations, and customizable controls, ensuring high performance and responsiveness.\n";

const char* multi_lingual_sample_text =
u8"کتابخانه LVGL یک مجموعه متن‌باز و سبک‌وزن برای توسعه رابط‌های کاربری گرافیکی پیشرفته در سیستم‌های تعبیه‌شده است. این کتابخانه با مدیریت مؤثر حافظه، پشتیبانی از لایه‌ها، انیمیشن‌ها و کنترل‌های قابل سفارشی‌سازی، کارایی و پاسخگویی بالا را فراهم می‌کند.\n"
"LVGL is an open-source, lightweight library designed to develop advanced graphical user interfaces on embedded systems. It efficiently manages memory, supports layering, animations, and customizable controls, ensuring high performance and responsiveness.\n";

const int c_fonts_count = 4;
static fonts_t c_fonts[] = {
    {.font_name = "Badr 24", .font= &badr_24, .text = &persian_sample_text},
    {.font_name = "Bardiya 24", .font = &bardiya_24, .text = &persian_sample_text},
    {.font_name = "Century Gothic 24", .font = &century_gothic_24, .text = &english_sample_text},
    {.font_name = "Segoei UI 24", .font = &segoei_ui_24, .text = &multi_lingual_sample_text},
};

const int symbol_fonts_count = 3;
static symbols_t symbol_fonts[] = {
    {.font_name = "Fontaw Asome 24", .font = &fontawesome_24, .symbols_count= TOTAL_FONTAWESOME_24_SYMBOLS, .symbols = fontawesome_24_symbol_table},
    {.font_name = "Material Icons 24", .font = &material_icons_24, .symbols_count = TOTAL_MATERIAL_ICONS_24_SYMBOLS, .symbols = material_icons_24_symbol_table},
    {.font_name = "Noto Emoji 24", .font = &noto_emoji_24, .symbols_count = TOTAL_NOTO_EMOJI_24_SYMBOLS, .symbols = noto_emoji_24_symbol_table},
};

const int bin_fonts_count = 3;
static bin_t bin_fonts[] = {
    {.font_name = "Roya 24", .path = "A:fonts/bin/roya_24.bin", .text = &persian_sample_text},
    {.font_name = "Merienda 24", .path = "A:fonts/bin/merienda_24.bin", .text = &english_sample_text},
    {.font_name = "Roboto 24", .path = "A:fonts/bin/roboto_24.bin", .text = &english_sample_text},
};

const int svg_fonts_count = 1;
static svgs_t svg_fonts[] = {
    {.font_name = "bootstrap_icons", .svgs_count = TOTAL_BOOTSTRAP_ICONS_SVGS, .max_svg_buffer = MAX_PATH_LENGTH_BOOTSTRAP_ICONS + MAX_TEMPLATE_LENGTH, .svgs = bootstrap_icons_svg_table},
};

static const char* svg_color_options = 
"aliceblue\n""antiquewhite\n""aqua\n""aquamarine\n""azure\n""beige\n""bisque\n""black\n""blanchedalmond\n""blue\n"
"blueviolet\n""brown\n""burlywood\n""cadetblue\n""chartreuse\n""chocolate\n""coral\n""cornflowerblue\n""cornsilk\n"
"crimson\n""cyan\n""darkblue\n""darkcyan\n""darkgoldenrod\n""darkgray\n""darkgrey\n""darkgreen\n""darkkhaki\n"
"darkmagenta\n""darkolivegreen\n""darkorange\n""darkorchid\n""darkred\n""darksalmon\n""darkseagreen\n"
"darkslateblue\n""darkslategray\n""darkslategrey\n""darkturquoise\n""darkviolet\n""deeppink\n""deepskyblue\n"
"dimgray\n""dimgrey\n""dodgerblue\n""firebrick\n""floralwhite\n""forestgreen\n""fuchsia\n""gainsboro\n""ghostwhite\n"
"gold\n""goldenrod\n""gray\n""grey\n""green\n""greenyellow\n""honeydew\n""hotpink\n""indianred\n""indigo\n""ivory\n"
"khaki\n""lavender\n""lavenderblush\n""lawngreen\n""lemonchiffon\n""lightblue\n""lightcoral\n""lightcyan\n"
"lightgoldenrodyellow\n""lightgray\n""lightgrey\n""lightgreen\n""lightpink\n""lightsalmon\n""lightseagreen\n"
"lightskyblue\n""lightslategray\n""lightslategrey\n""lightsteelblue\n""lightyellow\n""lime\n""limegreen\n""linen\n"
"magenta\n""maroon\n""mediumaquamarine\n""mediumblue\n""mediumorchid\n""mediumpurple\n""mediumseagreen\n"
"mediumslateblue\n""mediumspringgreen\n""mediumturquoise\n""mediumvioletred\n""midnightblue\n""mintcream\n"
"mistyrose\n""moccasin\n""navajowhite\n""navy\n""oldlace\n""olive\n""olivedrab\n""orange\n""orangered\n""orchid\n"
"palegoldenrod\n""palegreen\n""paleturquoise\n""palevioletred\n""papayawhip\n""peachpuff\n""peru\n""pink\n""plum\n"
"powderblue\n""purple\n""red\n""rosybrown\n""royalblue\n""saddlebrown\n""salmon\n""sandybrown\n""seagreen\n"
"seashell\n""sienna\n""silver\n""skyblue\n""slateblue\n""slategray\n""slategrey\n""snow\n""springgreen\n""steelblue\n"
"tan\n""teal\n""thistle\n""tomato\n""turquoise\n""violet\n""wheat\n""white\n""whitesmoke\n""yellow\n""yellowgreen";

static char svg_color[30]="aliceblue";
static float svg_opacity = 1.0;
static svg_def_t* current_svg = NULL;

static lv_style_t style_list_font = { 0 };
static lv_style_t style_list_lbl_normal = { 0 };
static lv_style_t style_list_lbl_checked = { 0 };
static lv_style_t style_symbol_container = { 0 };
static lv_style_t style_icon_container = { 0 };
static lv_style_t style_icon = { 0 };
static lv_style_t style_icon_label = { 0 };

static lv_obj_t* scr_main = NULL;
static lv_obj_t* tabview = NULL;
static lv_obj_t* tab1 = NULL;
static lv_obj_t* tab2 = NULL;
static lv_obj_t* tab3 = NULL;
static lv_obj_t* tab4 = NULL;

static lv_obj_t* c_font_textarea = NULL;
static lv_obj_t* symbol_container = NULL;
static lv_obj_t* bin_font_textarea = NULL;
static lv_font_t* bin_font = NULL;
static lv_obj_t* svg_image = NULL;

static void style_init(void);
static void create_tabview(lv_obj_t*);
static void create_tab1(lv_obj_t*);
static void create_tab2(lv_obj_t*);
static void create_tab3(lv_obj_t*);
static void create_tab4(lv_obj_t*);
static lv_obj_t* create_grid_container(lv_obj_t*, int32_t*, int32_t*);

static void list_c_font_label_event(lv_event_t*);
static void list_symbol_font_label_event(lv_event_t*);
static void list_bin_font_label_event(lv_event_t*);
static void on_svg_height_changed_event(lv_event_t*);
static void on_svg_color_changed_event(lv_event_t*);
static void on_svg_opacity_changed_event(lv_event_t*);
static void list_svg_label_event(lv_event_t*);
static void update_svg_event(lv_event_t*);

static char* get_svg(uint32_t id, float height, float x, float y, char* color, float opacity);

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

    style_init();

    scr_main = lv_obj_create(NULL);
    
    create_tabview(scr_main);
    create_tab1(tab1);
    create_tab2(tab2);
    create_tab3(tab3);
    create_tab4(tab4);
    
    lv_screen_load(scr_main);

    while (1)
    {
        //lv_task_handler();
        //lv_delay_ms(33);
        uint32_t time_till_next = lv_timer_handler();
        lv_delay_ms(time_till_next);
    }

    return 0;
}

static void style_init(void)
{
    lv_style_init(&style_list_font);
    lv_style_set_size(&style_list_font, lv_pct(100), lv_pct(100));
    lv_style_set_pad_row(&style_list_font, 5);
    lv_style_set_pad_top(&style_list_font, 5);
    lv_style_set_pad_bottom(&style_list_font, 5);
    lv_style_set_bg_opa(&style_list_font, LV_OPA_40);
    lv_style_set_bg_color(&style_list_font, lv_color_hex(0xFDFDFD));
    lv_style_set_margin_all(&style_list_font, 2);
    lv_style_set_align(&style_list_font, LV_ALIGN_LEFT_MID);
    lv_style_set_x(&style_list_font, 2);

    lv_style_init(&style_list_lbl_normal);
    lv_style_set_bg_opa(&style_list_lbl_normal, LV_OPA_40);
    lv_style_set_bg_color(&style_list_lbl_normal, lv_color_hex(0xDDDDDD));
    lv_style_set_pad_left(&style_list_lbl_normal, 12);
    lv_style_set_pad_right(&style_list_lbl_normal, 12);
    lv_style_set_pad_top(&style_list_lbl_normal, 12);
    lv_style_set_size(&style_list_lbl_normal, lv_pct(100), 40);
    lv_style_set_text_align(&style_list_lbl_normal, LV_TEXT_ALIGN_LEFT);
    lv_style_set_radius(&style_list_lbl_normal, 12);

    lv_style_init(&style_list_lbl_checked);
    lv_style_set_bg_color(&style_list_lbl_checked, lv_color_hex(0xFF0000));

    lv_style_init(&style_symbol_container);
    lv_style_set_flex_flow(&style_symbol_container, LV_FLEX_FLOW_ROW_WRAP);
    lv_style_set_flex_main_place(&style_symbol_container, LV_FLEX_ALIGN_START);
    lv_style_set_flex_cross_place(&style_symbol_container, LV_FLEX_ALIGN_CENTER);
    lv_style_set_layout(&style_symbol_container, LV_LAYOUT_FLEX);

    lv_style_set_width(&style_icon_container, LV_SIZE_CONTENT);
    lv_style_set_height(&style_icon_container, LV_SIZE_CONTENT);
    lv_style_set_layout(&style_icon_container, LV_LAYOUT_FLEX);
    lv_style_set_flex_flow(&style_icon_container, LV_FLEX_FLOW_COLUMN);
    lv_style_set_flex_main_place(&style_icon_container, LV_FLEX_ALIGN_CENTER);
    lv_style_set_flex_cross_place(&style_icon_container, LV_FLEX_ALIGN_CENTER);
    lv_style_set_pad_all(&style_icon_container, 5);
    lv_style_set_pad_row(&style_icon_container, 2);

    lv_style_init(&style_icon);
    lv_style_set_width(&style_icon, 60);
    lv_style_set_height(&style_icon, 35);
    lv_style_set_text_align(&style_icon, LV_TEXT_ALIGN_CENTER);

    lv_style_init(&style_icon_label);
    lv_style_set_width(&style_icon_label, 60);
    lv_style_set_height(&style_icon_label, LV_SIZE_CONTENT);
    lv_style_set_text_align(&style_icon_label, LV_TEXT_ALIGN_CENTER);
    lv_style_set_text_font(&style_icon_label, &lv_font_montserrat_14);
}

static void create_tabview(lv_obj_t* parent)
{
    tabview = lv_tabview_create(parent);

    tab1 = lv_tabview_add_tab(tabview, TAB1_TITLE);
    tab2 = lv_tabview_add_tab(tabview, TAB2_TITLE);
    tab3 = lv_tabview_add_tab(tabview, TAB3_TITLE);
    tab4 = lv_tabview_add_tab(tabview, TAB4_TITLE);

    lv_obj_t* tab_bar = lv_tabview_get_tab_bar(tabview);
    lv_obj_set_style_pad_left(tab_bar, 220, 0);
    lv_obj_t* logo = lv_image_create(tab_bar);
    lv_obj_add_flag(logo, LV_OBJ_FLAG_IGNORE_LAYOUT);
    LV_IMAGE_DECLARE(img_tab_logo);
    lv_image_set_src(logo, &img_tab_logo);
    lv_obj_align(logo, LV_ALIGN_LEFT_MID, -210, 0);
}

static void create_tab1(lv_obj_t* parent)
{
    static int32_t col_dsc[] = { LV_GRID_FR(2),10, LV_GRID_FR(5), LV_GRID_TEMPLATE_LAST};
    static int32_t row_dsc[] = { LV_GRID_FR(1), LV_GRID_TEMPLATE_LAST };
    lv_obj_t* tab1_container = create_grid_container(parent, col_dsc, row_dsc);

    lv_obj_t*  c_fonts_list = lv_list_create(tab1_container);
    lv_obj_add_style(c_fonts_list, &style_list_font, 0);

    lv_obj_set_grid_cell(c_fonts_list, LV_GRID_ALIGN_STRETCH, 0, 1, LV_GRID_ALIGN_STRETCH, 0, 1);

    /*Add labels to the list*/
    lv_obj_t* lbl_c_font;

    int i;

    for (i = 0; i < c_fonts_count; i++)
    {
        lbl_c_font = lv_label_create(c_fonts_list);
        lv_obj_add_flag(lbl_c_font, LV_OBJ_FLAG_CLICKABLE);
        
        lv_label_set_text_fmt(lbl_c_font, c_fonts[i].font_name);
        lv_label_set_long_mode(lbl_c_font, LV_LABEL_LONG_MODE_SCROLL);
        lv_obj_add_style(lbl_c_font, &style_list_lbl_normal, 0);
        if (i == 0)
        {
            lv_obj_add_style(lbl_c_font, &style_list_lbl_checked, 0);
        }
        lv_obj_add_event_cb(lbl_c_font, list_c_font_label_event, LV_EVENT_CLICKED, NULL);

    }
    
    c_font_textarea = lv_textarea_create(tab1_container);
    lv_obj_set_width(c_font_textarea, lv_pct(80));
    lv_obj_set_height(c_font_textarea, lv_pct(80));
    lv_obj_set_align(c_font_textarea, LV_ALIGN_CENTER);
    lv_obj_set_style_base_dir(c_font_textarea, LV_BASE_DIR_AUTO, 0);
    lv_obj_set_style_text_align(c_font_textarea, LV_TEXT_ALIGN_AUTO, 0);
    lv_textarea_add_text(c_font_textarea, *c_fonts[0].text);
    lv_obj_set_style_text_font(c_font_textarea, c_fonts[0].font, 0);
    lv_obj_set_grid_cell(c_font_textarea, LV_GRID_ALIGN_STRETCH, 2, 1, LV_GRID_ALIGN_STRETCH, 0, 1);
}

static void create_tab2(lv_obj_t* parent)
{
    static int32_t col_dsc[] = { LV_GRID_FR(2),10, LV_GRID_FR(5), LV_GRID_TEMPLATE_LAST };
    static int32_t row_dsc[] = { LV_GRID_FR(1), LV_GRID_TEMPLATE_LAST };
    lv_obj_t* tab2_container = create_grid_container(parent, col_dsc, row_dsc);

    lv_obj_t* symbol_fonts_list = lv_list_create(tab2_container);
    lv_obj_add_style(symbol_fonts_list, &style_list_font, 0);

    lv_obj_set_grid_cell(symbol_fonts_list, LV_GRID_ALIGN_STRETCH, 0, 1, LV_GRID_ALIGN_STRETCH, 0, 1);

    /*Add labels to the list*/
    lv_obj_t* lbl_symbol_font;

    int i;

    for (i = 0; i < symbol_fonts_count; i++)
    {
        lbl_symbol_font = lv_label_create(symbol_fonts_list);
        lv_obj_add_flag(lbl_symbol_font, LV_OBJ_FLAG_CLICKABLE);

        lv_label_set_text_fmt(lbl_symbol_font, symbol_fonts[i].font_name);
        lv_label_set_long_mode(lbl_symbol_font, LV_LABEL_LONG_MODE_SCROLL);
        lv_obj_add_style(lbl_symbol_font, &style_list_lbl_normal, 0);
        if (i == 0)
        {
            lv_obj_add_style(lbl_symbol_font, &style_list_lbl_checked, 0);
        }
        lv_obj_add_event_cb(lbl_symbol_font, list_symbol_font_label_event, LV_EVENT_CLICKED, NULL);

    }

    symbol_container = lv_obj_create(tab2_container);
    lv_obj_set_width(symbol_container, lv_pct(80));
    lv_obj_set_height(symbol_container, lv_pct(80));
    lv_obj_set_align(symbol_container, LV_ALIGN_CENTER);
    lv_obj_set_style_text_font(symbol_container, c_fonts[0].font, 0);
    lv_obj_add_style(symbol_container, &style_symbol_container, 0);
    lv_obj_set_style_text_font(symbol_container, symbol_fonts[0].font, 0);
    lv_obj_set_grid_cell(symbol_container, LV_GRID_ALIGN_STRETCH, 2, 1, LV_GRID_ALIGN_STRETCH, 0, 1);

    const symbols_t* font = &symbol_fonts[0];
    const symbol_def_t* symbol = font->symbols;
    while (symbol->name != NULL) {
        lv_obj_t* icon_container = lv_obj_create(symbol_container);
        lv_obj_add_style(icon_container, &style_icon_container, 0);

        lv_obj_t* icon = lv_label_create(icon_container);
        lv_obj_add_style(icon, &style_icon, 0);
        lv_label_set_text(icon, symbol->value);

        lv_obj_t* icon_label = lv_label_create(icon_container);
        lv_obj_add_style(icon_label, &style_icon_label, 0);
        lv_label_set_text(icon_label, symbol->name);
        lv_label_set_long_mode(icon_label, LV_LABEL_LONG_SCROLL);

        symbol++;
    }

}

static void create_tab3(lv_obj_t* parent)
{
    static int32_t col_dsc[] = { LV_GRID_FR(2),10, LV_GRID_FR(5), LV_GRID_TEMPLATE_LAST };
    static int32_t row_dsc[] = { LV_GRID_FR(1), LV_GRID_TEMPLATE_LAST };
    lv_obj_t* tab3_container = create_grid_container(parent, col_dsc, row_dsc);

    lv_obj_t* c_fonts_list = lv_list_create(tab3_container);
    lv_obj_add_style(c_fonts_list, &style_list_font, 0);

    lv_obj_set_grid_cell(c_fonts_list, LV_GRID_ALIGN_STRETCH, 0, 1, LV_GRID_ALIGN_STRETCH, 0, 1);

    /*Add labels to the list*/
    lv_obj_t* lbl_bin_font;

    int i;

    for (i = 0; i < bin_fonts_count; i++)
    {
        lbl_bin_font = lv_label_create(c_fonts_list);
        lv_obj_add_flag(lbl_bin_font, LV_OBJ_FLAG_CLICKABLE);

        lv_label_set_text_fmt(lbl_bin_font, bin_fonts[i].font_name);
        lv_label_set_long_mode(lbl_bin_font, LV_LABEL_LONG_MODE_SCROLL);
        lv_obj_add_style(lbl_bin_font, &style_list_lbl_normal, 0);
        if (i == 0)
        {
            lv_obj_add_style(lbl_bin_font, &style_list_lbl_checked, 0);
        }
        lv_obj_add_event_cb(lbl_bin_font, list_bin_font_label_event, LV_EVENT_CLICKED, NULL);

    }

    bin_font = lv_binfont_create(bin_fonts[0].path);
    
    bin_font_textarea = lv_textarea_create(tab3_container);
    lv_obj_set_width(bin_font_textarea, lv_pct(80));
    lv_obj_set_height(bin_font_textarea, lv_pct(80));
    lv_obj_set_align(bin_font_textarea, LV_ALIGN_CENTER);
    lv_obj_set_style_base_dir(bin_font_textarea, LV_BASE_DIR_AUTO, 0);
    lv_obj_set_style_text_align(bin_font_textarea, LV_TEXT_ALIGN_AUTO, 0);
    lv_textarea_add_text(bin_font_textarea, *bin_fonts[0].text);
    lv_obj_set_style_text_font(bin_font_textarea, bin_font, 0);
    lv_obj_set_grid_cell(bin_font_textarea, LV_GRID_ALIGN_STRETCH, 2, 1, LV_GRID_ALIGN_STRETCH, 0, 1);
}

static void create_tab4(lv_obj_t* parent)
{
    static int32_t col_dsc[] = { LV_GRID_FR(2),10, LV_GRID_FR(5), LV_GRID_TEMPLATE_LAST };
    static int32_t row_dsc[] = { 40, 10, 40, 10, 40, 10, LV_GRID_FR(1), LV_GRID_TEMPLATE_LAST };
    lv_obj_t* tab4_container = create_grid_container(parent, col_dsc, row_dsc);

    lv_obj_t* svg_height_text = lv_textarea_create(tab4_container);
    lv_obj_center(svg_height_text);
    lv_textarea_set_one_line(svg_height_text, true);
    lv_obj_align(svg_height_text, LV_ALIGN_CENTER, 0, 0);
    lv_obj_set_style_pad_top(svg_height_text, 10, 0);
    lv_obj_set_style_pad_bottom(svg_height_text, 10, 0);
    lv_textarea_set_accepted_chars(svg_height_text, "0123456789");
    lv_textarea_set_max_length(svg_height_text, 3);
    lv_textarea_set_text(svg_height_text, "400");
    lv_obj_set_grid_cell(svg_height_text, LV_GRID_ALIGN_STRETCH, 0, 1, LV_GRID_ALIGN_STRETCH, 0, 1);
    lv_obj_add_event_cb(svg_height_text, on_svg_height_changed_event, LV_EVENT_ALL, NULL);

    lv_obj_t* svg_colors_list = lv_dropdown_create(tab4_container);
    lv_obj_center(svg_colors_list);
    lv_dropdown_set_options_static(svg_colors_list, svg_color_options);
    lv_obj_add_event_cb(svg_colors_list, on_svg_color_changed_event, LV_EVENT_VALUE_CHANGED, NULL);
    lv_obj_set_grid_cell(svg_colors_list, LV_GRID_ALIGN_STRETCH, 0, 1, LV_GRID_ALIGN_STRETCH, 2, 1);

    lv_obj_t* svg_opacity_slider = lv_slider_create(tab4_container);
    lv_obj_center(svg_opacity_slider);
    lv_obj_set_width(svg_opacity_slider, lv_pct(20));
    lv_slider_set_value(svg_opacity_slider, 100, LV_ANIM_OFF);
    lv_obj_set_grid_cell(svg_opacity_slider, LV_GRID_ALIGN_CENTER, 0, 1, LV_GRID_ALIGN_CENTER, 4, 1);
    lv_obj_add_event_cb(svg_opacity_slider, on_svg_opacity_changed_event, LV_EVENT_VALUE_CHANGED, NULL);

    lv_obj_t* svg_list = lv_list_create(tab4_container);
    lv_obj_add_style(svg_list, &style_list_font, 0);
    lv_obj_set_grid_cell(svg_list, LV_GRID_ALIGN_STRETCH, 0, 1, LV_GRID_ALIGN_STRETCH, 6, 1);

    /*Add labels to the list*/
    lv_obj_t* lbl_svg;
    int i;
    for (i = 0; i < svg_fonts[0].svgs_count; i++)
    {
        lbl_svg = lv_label_create(svg_list);
        lv_obj_add_flag(lbl_svg, LV_OBJ_FLAG_CLICKABLE);

        lv_label_set_text_fmt(lbl_svg, svg_fonts[0].svgs[i].name);
        lv_label_set_long_mode(lbl_svg, LV_LABEL_LONG_MODE_SCROLL);
        lv_obj_add_style(lbl_svg, &style_list_lbl_normal, 0);
        if (i == 0)
        {
            lv_obj_add_style(lbl_svg, &style_list_lbl_checked, 0);
            current_svg = &svg_fonts[0].svgs[i];
        }
        lv_obj_add_event_cb(lbl_svg, list_svg_label_event, LV_EVENT_CLICKED, NULL);
    }

    lv_obj_t* svg_container = lv_obj_create(tab4_container);
    lv_obj_set_width(svg_container, lv_pct(100));
    lv_obj_set_height(svg_container, lv_pct(100));
    lv_obj_set_align(svg_container, LV_ALIGN_CENTER);
    lv_obj_set_grid_cell(svg_container, LV_GRID_ALIGN_STRETCH, 2, 1, LV_GRID_ALIGN_STRETCH, 0, 7);
    
    svg_image = lv_obj_create(svg_container);
    lv_obj_remove_style_all(svg_image);
    lv_obj_center(svg_image);
    lv_obj_set_style_pad_all(svg_image, 5, 0);
    lv_obj_set_height(svg_image, 400);
    lv_obj_set_width(svg_image, lv_pct(100));
    lv_obj_set_style_opa(svg_image, LV_OPA_50, 0);
    lv_obj_add_event_cb(svg_image, update_svg_event, LV_EVENT_ALL, NULL);
}

static lv_obj_t* create_grid_container(lv_obj_t* parent, int32_t* col_dsc, int32_t* row_dsc)
{
    lv_obj_t* container = lv_obj_create(parent);
    lv_obj_remove_style_all(container);
    lv_obj_set_style_grid_column_dsc_array(container, col_dsc, 0);
    lv_obj_set_style_grid_row_dsc_array(container, row_dsc, 0);
    lv_obj_set_style_bg_opa(container, LV_OPA_0, 0);
    lv_obj_set_size(container, lv_pct(100), lv_pct(100));
    lv_obj_center(container);
    lv_obj_set_layout(container, LV_LAYOUT_GRID);
    return container;
}

// C Fonts Events
static void list_c_font_label_event(lv_event_t* e)
{
    lv_event_code_t code = lv_event_get_code(e);
    lv_obj_t* obj = lv_event_get_target(e);
    if (code == LV_EVENT_CLICKED)
    {
        lv_obj_t* parent = lv_obj_get_parent(obj);
        uint32_t i;
        for (i = 0; i < lv_obj_get_child_count(parent); i++)
        {
            lv_obj_t* child = lv_obj_get_child(parent, i);
            lv_obj_remove_style_all(child);
            lv_obj_add_style(child, &style_list_lbl_normal, 0);
            if (child == obj)
            {
                lv_obj_add_style(child, &style_list_lbl_checked, 0);
                lv_obj_set_style_text_font(c_font_textarea, c_fonts[i].font, 0);
                lv_textarea_set_text(c_font_textarea, *c_fonts[i].text);
            }
        }
    }
}

// Symbol Fonts Events
static void list_symbol_font_label_event(lv_event_t* e)
{
    lv_event_code_t code = lv_event_get_code(e);
    lv_obj_t* obj = lv_event_get_target(e);
    if (code == LV_EVENT_CLICKED)
    {
        lv_obj_t* parent = lv_obj_get_parent(obj);
        uint32_t i;
        for (i = 0; i < lv_obj_get_child_count(parent); i++)
        {
            lv_obj_t* child = lv_obj_get_child(parent, i);
            lv_obj_remove_style_all(child);
            lv_obj_add_style(child, &style_list_lbl_normal, 0);
            if (child == obj)
            {
                lv_obj_clean(symbol_container);

                lv_obj_add_style(child, &style_list_lbl_checked, 0);
                lv_obj_set_style_text_font(symbol_container, symbol_fonts[i].font, 0);

                
               
                const symbols_t* font = &symbol_fonts[i];
                const symbol_def_t* symbol = font->symbols;
                while (symbol->name != NULL) {
                    lv_obj_t* icon_container = lv_obj_create(symbol_container);
                    lv_obj_add_style(icon_container, &style_icon_container, 0);

                    lv_obj_t* icon = lv_label_create(icon_container);
                    lv_obj_add_style(icon, &style_icon, 0);
                    lv_label_set_text(icon, symbol->value);

                    lv_obj_t* icon_label = lv_label_create(icon_container);
                    lv_obj_add_style(icon_label, &style_icon_label, 0);
                    lv_label_set_text(icon_label, symbol->name);
                    lv_label_set_long_mode(icon_label, LV_LABEL_LONG_SCROLL);

                    symbol++;
                }

            }
        }
        

    }
}

// Binary Fonts Events
static void list_bin_font_label_event(lv_event_t* e)
{
    lv_event_code_t code = lv_event_get_code(e);
    lv_obj_t* obj = lv_event_get_target(e);
    if (code == LV_EVENT_CLICKED)
    {
        lv_obj_t* parent = lv_obj_get_parent(obj);
        uint32_t i;
        for (i = 0; i < lv_obj_get_child_count(parent); i++)
        {
            lv_obj_t* child = lv_obj_get_child(parent, i);
            lv_obj_remove_style_all(child);
            lv_obj_add_style(child, &style_list_lbl_normal, 0);
            if (child == obj)
            {
                lv_obj_add_style(child, &style_list_lbl_checked, 0);
                lv_binfont_destroy(bin_font);
                bin_font = lv_binfont_create(bin_fonts[i].path);
                lv_obj_set_style_text_font(bin_font_textarea, bin_font, 0);
                lv_textarea_set_text(bin_font_textarea, *bin_fonts[i].text);
            }
        }
    }
}

// SVG Fonts Events
static void on_svg_height_changed_event(lv_event_t* e)
{
    lv_event_code_t code = lv_event_get_code(e);
    lv_obj_t* ta = lv_event_get_target(e);
    const int32_t max_value = 400;
    const int32_t min_value = 0;
    const int32_t step_value = 1;
    const char* text_value = lv_textarea_get_text(ta);
    int32_t value = 0;
    if (code == LV_EVENT_VALUE_CHANGED) 
    {
        if (text_value == NULL || strlen(text_value) == 0)
        {
            lv_obj_set_height(svg_image, max_value);
            return;
        }
        value = (int32_t)atoi(text_value);
        if (value > max_value)
        {
            char buf[8];
            snprintf(buf, sizeof(buf), "%d", max_value);
            if (strcmp(text_value, buf) != 0) 
            {
                lv_textarea_set_text(ta, buf);
                lv_textarea_set_cursor_pos(ta, LV_TEXTAREA_CURSOR_LAST);
                lv_obj_set_height(svg_image, max_value);
            }
            return;
        }
        lv_obj_set_height(svg_image, value);
    }
    else if (code == LV_EVENT_KEY)
    {
        uint32_t key = lv_event_get_key(e);

        if (text_value == NULL || strlen(text_value) == 0)
        {
            value = 0;
        }
        else
        {
            value = atoi(text_value);
        }

        if (key == LV_KEY_UP) 
        {
            if (value < max_value) value += step_value;
        }
        else if (key == LV_KEY_DOWN) {
            if (value > min_value) value -= step_value;
        }
        else 
        {
            return;
        }
        char buf[8];
        snprintf(buf, sizeof(buf), "%d", value);
        lv_textarea_set_text(ta, buf);
        lv_textarea_set_cursor_pos(ta, LV_TEXTAREA_CURSOR_LAST);
    }
}

static void on_svg_color_changed_event(lv_event_t* e)
{
    lv_event_code_t code = lv_event_get_code(e);
    lv_obj_t* ddl = lv_event_get_target(e);

    if (code == LV_EVENT_VALUE_CHANGED && ddl != NULL)
    {
        lv_dropdown_get_selected_str(ddl, svg_color, 29);
        lv_obj_invalidate(svg_image);
    }
}

static void on_svg_opacity_changed_event(lv_event_t* e)
{
    lv_event_code_t code = lv_event_get_code(e);
    lv_obj_t* slider = lv_event_get_target(e);

    if (code == LV_EVENT_VALUE_CHANGED && slider != NULL)
    {
        int32_t value = lv_slider_get_value(slider);
        svg_opacity = (float)value / 100.0;
        lv_obj_invalidate(svg_image);
    }
}

static void list_svg_label_event(lv_event_t* e)
{
    lv_event_code_t code = lv_event_get_code(e);
    lv_obj_t* obj = lv_event_get_target(e);
    if (code == LV_EVENT_CLICKED)
    {
        lv_obj_t* parent = lv_obj_get_parent(obj);
        uint32_t i;
        for (i = 0; i < lv_obj_get_child_count(parent); i++)
        {
            lv_obj_t* child = lv_obj_get_child(parent, i);
            lv_obj_remove_style_all(child);
            lv_obj_add_style(child, &style_list_lbl_normal, 0);
            if (child == obj)
            {
                lv_obj_add_style(child, &style_list_lbl_checked, 0);
                current_svg = &svg_fonts[0].svgs[i];
                lv_obj_invalidate(svg_image);
            }
        }
    }
}

static void update_svg_event(lv_event_t* e)
{
    lv_event_code_t code = lv_event_get_code(e);
    lv_layer_t* layer = lv_event_get_layer(e);
    lv_obj_t* obj = lv_event_get_target(e);
    if (layer == NULL)
        return;

    if (code == LV_EVENT_DRAW_MAIN || code == LV_EVENT_SIZE_CHANGED)
    {
        lv_area_t layer_cords = layer->buf_area;

        lv_area_t coords;
        lv_obj_get_coords(obj, &coords);

        float width = coords.x2 - coords.x1;
        float height = coords.y2 - coords.y1;
        
        float obj_x_center = width / 2.0;
        float obj_y_center = height / 2.0;

        float x = coords.x1 + obj_x_center;
        float y = coords.y1 + obj_y_center;

        char* svg_data = get_svg(0, height, x, y, svg_color, svg_opacity);

        lv_svg_node_t* svg_node = lv_svg_load_data(svg_data, strlen(svg_data));
        if (!svg_node) return;

        lv_draw_svg(layer, svg_node);
        lv_svg_node_delete(svg_node);
    }
}

char* get_svg(uint32_t id, float height, float x, float y, const char* color, float opacity)
{
    char* svg_data = malloc(MAX_TEMPLATE_LENGTH + MAX_PATH_LENGTH_BOOTSTRAP_ICONS);
    if (svg_data == NULL) return;

    float vb_width = current_svg->width;
    float vb_height = current_svg->height;
    float tanslate_x = current_svg->offset_x;
    float translate_y = current_svg->offset_y;

    float svg_scale = vb_height / height;
    float width = vb_width / svg_scale;
    float vb_x = (vb_width / 2.0) - (x * svg_scale);
    float vb_y = (vb_height / 2.0) - (y * svg_scale);

    sprintf(svg_data, svg_template, width, height, vb_x, vb_y, vb_width, vb_height, tanslate_x, translate_y, current_svg->path, color, opacity);

    return svg_data;
}

