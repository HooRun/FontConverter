window.getElementBoundingClientRect = async function (element) {
    try {
        if (!element || typeof element.getBoundingClientRect !== 'function') {
            console.error('Invalid element provided to getElementBoundingClientRect');
            return { width: 0, height: 0 };
        }
        const rect = element.getBoundingClientRect();
        return {
            width: rect.width,
            height: rect.height
        };
    } catch (error) {
        console.error('Error in getElementBoundingClientRect:', error);
        return { width: 0, height: 0 };
    }
};