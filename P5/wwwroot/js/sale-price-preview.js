// Aperçu du prix de vente pendant la saisie du prix d'achat.
// Simple confort d'affichage : le serveur recalcule toujours le prix, rien de ceci n'est envoyé.
(function () {
    const box = document.querySelector('[data-sale-price-preview]');
    const input = document.getElementById('PurchasePrice');
    if (!box || !input) {
        return;
    }

    const output = box.querySelector('output');
    const repairsCost = Number(box.dataset.repairsCost) || 0;
    const margin = Number(box.dataset.margin) || 0;
    const euros = new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR' });

    // La saisie est française : « 10 990,50 ». On retire les espaces et on remplace la virgule.
    function parsePrice(text) {
        const cleaned = text.replace(/[\s\u00a0\u202f]/g, '').replace(',', '.');
        return cleaned === '' ? NaN : Number(cleaned);
    }

    function refresh() {
        const price = parsePrice(input.value);
        output.textContent = Number.isFinite(price) && price >= 0
            ? euros.format(price + repairsCost + margin)
            : '—';
    }

    input.addEventListener('input', refresh);
    refresh();
})();
