(() => {
    if (document.readyState === 'loading')
        document.addEventListener('DOMContentLoaded', init)
    else
        init()

    function init() {
        const nav = document.querySelector('.navigation')
        if (!nav)
            return
        const CLASS = 'mouse-focus'

        function clearAll() {
            document.querySelectorAll('.navigation-item.mouse-focus').forEach(el => el.classList.remove('mouse-focus'))
        }

        document.addEventListener('pointerdown', (e) => {
            if (e.pointerType !== 'mouse')
                return

            const clickedItem = e.target.closest('.navigation-item')

            if (!clickedItem) {
                clearAll()
                return
            }

            clearAll()

            if (clickedItem.classList.contains(CLASS))
                clearAll()
            else
                clickedItem.classList.add(CLASS)
        });

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape')
                clearAll()
        })

        nav.addEventListener('pointerover', (e) => {
            if (e.pointerType && e.pointerType !== 'mouse')
                return

            const hoveredItem = e.target.closest('.navigation-item')
            if (!hoveredItem)
                return

            const focused = document.querySelector('.navigation-item.' + CLASS)
            if (focused && focused !== hoveredItem)
                clearAll()
        })
    }
})()