// Timeline Modal JavaScript
class TimelineModal {
    constructor() {
        this.offcanvasElement = document.getElementById('timelineOffcanvas');
        this.offcanvas = null;
    }

    async show(volunteerId, volunteerName) {
        this.setTitle(volunteerName);
        this.initializeModal();
        this.showModal();
        await this.loadTimeline(volunteerId);
    }

    setTitle(volunteerName) {
        const titleElement = document.getElementById('timelineName');
        titleElement.textContent = `${volunteerName} - Hareket Geçmişi`;
    }

    initializeModal() {
        if (!this.offcanvas) {
            this.offcanvas = new bootstrap.Offcanvas(this.offcanvasElement);
        }
        this.applyTheme();
    }

    applyTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const body = document.body;
        
        if (currentTheme === 'dark') {
            this.offcanvasElement.setAttribute('data-bs-theme', 'dark');
            body.setAttribute('data-theme', 'dark');
        } else {
            this.offcanvasElement.removeAttribute('data-bs-theme');
        }
    }

    showModal() {
        this.offcanvas.show();
    }

    async loadTimeline(volunteerId) {
        const contentElement = document.getElementById('timelineContent');
        
        try {
            const response = await fetch(`/volunteers/timeline/${volunteerId}`);
            if (!response.ok) throw new Error('Network response was not ok');
            
            const movements = await response.json();
            contentElement.innerHTML = this.generateTimelineHtml(movements);
        } catch (error) {
            console.error('Timeline load error:', error);
            contentElement.innerHTML = this.getErrorHtml();
        }
    }

    generateTimelineHtml(movements) {
        if (movements.length === 0) {
            return this.getEmptyStateHtml();
        }

        const timelineItems = movements.map(movement => this.createTimelineItem(movement)).join('');
        return `<div class="timeline">${timelineItems}</div>`;
    }

    createTimelineItem(movement) {
        const badges = this.createBadges(movement);
        const notes = movement.notes ? this.createNotes(movement.notes) : '';
        
        return `
            <div class="timeline-item">
                <div class="d-flex align-items-start">
                    <div class="timeline-time text-muted me-3">${movement.timeFormatted}</div>
                    <div class="timeline-icon-wrapper me-3 d-flex align-items-center justify-content-center">
                        <i class="${movement.timelineIcon}"></i>
                    </div>
                    <div class="timeline-content flex-grow-1">
                        <div class="fw-semibold text-body mb-1">${movement.movementDescription}</div>
                        <div class="d-flex flex-wrap gap-2 mb-2">${badges}</div>
                        ${notes}
                    </div>
                </div>
            </div>
        `;
    }

    createBadges(movement) {
        let badges = `<span class="badge bg-secondary-subtle text-secondary-emphasis">${movement.movementType}</span>`;
        if (movement.isGroupMovement) {
            badges += '<span class="badge bg-info-subtle text-info-emphasis">Grup</span>';
        }
        return badges;
    }

    createNotes(notes) {
        return `<div class="small text-info-emphasis bg-info-subtle px-2 py-1 rounded">${notes}</div>`;
    }

    getEmptyStateHtml() {
        return `
            <div class="text-center text-muted p-4">
                <i class="bi bi-info-circle fs-1 mb-3"></i>
                <h6>Henüz hareket kaydı bulunmuyor</h6>
                <p class="small mb-0">Bu ekip üyesi için henüz hareket geçmişi mevcut değil.</p>
            </div>
        `;
    }

    getErrorHtml() {
        return `
            <div class="alert alert-danger">
                <i class="bi bi-exclamation-triangle me-2"></i>
                <strong>Hata:</strong> Hareket geçmişi yüklenemedi. Lütfen tekrar deneyin.
            </div>
        `;
    }
}

// Global timeline instance
const timelineModal = new TimelineModal();

// Global function for compatibility
function showTimeline(volunteerId, volunteerName) {
    timelineModal.show(volunteerId, volunteerName);
}

// Volunteer deletion function
function deleteVolunteer(id) {
    if (confirm('Bu ekip üyesini silmek istediğinizden emin misiniz?')) {
        fetch(`/volunteers/delete/${id}`, {
            method: 'POST'
        }).then(response => {
            if (response.ok) {
                location.reload();
            } else {
                alert('Silme işlemi başarısız oldu.');
            }
        }).catch(error => {
            console.error('Delete error:', error);
            alert('Bir hata oluştu.');
        });
    }
}
