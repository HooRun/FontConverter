
window.getElementDimensions = (selector) => {
    try {
        const element = document.querySelector(selector);
        if (!element) {
            return {
                Success: false,
                Error: `Element with selector '${selector}' not found`,
                Width: 0,
                Height: 0
            };
        }
        return {
            Success: true,
            Width: element.offsetWidth,
            Height: element.offsetHeight
        };
    } catch (error) {
        return {
            Success: false,
            Error: `An error occurred: '${error.message}'`,
            Width: 0,
            Height: 0
        };
    }
};

window.elementResizeObserver = {
    observers: {},

    observe: function (elementId, dotNetHelper, callbackMethodName) {
        try {
            const element = document.getElementById(elementId);
            if (!element) {
                dotNetHelper.invokeMethodAsync(callbackMethodName, {
                    Success: false,
                    Error: `Element with id '${elementId}' not found`,
                    Width: 0,
                    Height: 0
                });
                return;
            }

            const observer = new ResizeObserver(entries => {
                for (let entry of entries) {
                    const rect = entry.contentRect;
                    dotNetHelper.invokeMethodAsync(callbackMethodName, {
                        Success: true,
                        Error: null,
                        Width: rect.width,
                        Height: rect.height
                    });
                }
            });

            observer.observe(element);
            this.observers[elementId] = observer;
        } catch (error) {
            dotNetHelper.invokeMethodAsync(callbackMethodName, {
                Success: false,
                Error: `An error occurred: ${error.message}`,
                Width: 0,
                Height: 0
            });
        }
    },

    unobserve: function (elementId) {
        const observer = this.observers[elementId];
        if (observer) {
            observer.disconnect();
            delete this.observers[elementId];
        }
    }
};
