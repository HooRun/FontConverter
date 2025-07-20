// Get Element Size
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


// Element Resize Observer 
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



// Glyph Visibility Observer Manager
window._glyphVisibilityObservers = window._glyphVisibilityObservers || {};

window.startGlyphVisibilityTracking = (element, dotNetRef, glyphId) => {
    if (!('IntersectionObserver' in window)) return;
    if (!element || !element.isConnected) return;

    if (window._glyphVisibilityObservers[glyphId]) {
        window.stopGlyphVisibilityTracking(glyphId);
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            const el = entry.target;
            const isInDOM = document.body.contains(el);

            if (!isInDOM) return;

            if (entry.isIntersecting) {
                dotNetRef.invokeMethodAsync('OnVisible', glyphId);
            } else {
                dotNetRef.invokeMethodAsync('OnInvisible', glyphId);
            }
        });
    }, { root: null, threshold: 0.1 });

    observer.observe(element);
    window._glyphVisibilityObservers[glyphId] = observer;
};

window.stopGlyphVisibilityTracking = (glyphId) => {
    const observer = window._glyphVisibilityObservers[glyphId];
    if (observer) {
        observer.disconnect();
        delete window._glyphVisibilityObservers[glyphId];
    }
};


window.copyTextToClipboard = function (text) {
    if (navigator.clipboard) {
        return navigator.clipboard.writeText(text);
    } else {
        const textarea = document.createElement("textarea");
        textarea.value = text;
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand("copy");
        document.body.removeChild(textarea);
        return Promise.resolve();
    }
};

window.downloadFileFromBytes = function (fileName, contentBytesBase64) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + contentBytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
