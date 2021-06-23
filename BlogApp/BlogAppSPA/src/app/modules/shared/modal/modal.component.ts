import {
  Component, Input, OnInit, ViewChild,
} from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css'],
})
export class ModalComponent implements OnInit {
  @Input() item!: string;

  @ViewChild('content', { static: false }) private content!: { nativeElement: string; };

  constructor(private modalService: NgbModal) { }

  ngOnInit(): void {
  }

  async open() {
    let submited = false;
    await this.modalService.open(this.content).result.then(() => {
      submited = true;
    }, () => {
    });

    return submited;
  }
}
