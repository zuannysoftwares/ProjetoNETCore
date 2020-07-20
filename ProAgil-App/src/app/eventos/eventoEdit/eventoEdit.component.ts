import { Component, OnInit } from '@angular/core';
import { EventoService } from 'src/app/_services/evento.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';
import { Evento } from 'src/app/_models/Evento';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-evento-edit',
  templateUrl: './eventoEdit.component.html',
  styleUrls: ['./eventoEdit.component.css']
})
export class EventoEditComponent implements OnInit {

  titulo = 'Editar Evento';
  evento: Evento = new Evento();
  imagemURL = 'assets/img/upload.jpeg';
  registerForm: FormGroup;
  file: File;
  fileNameToUpdate: string;

  dataAtual: any;

  get lotes(): FormArray{
    return <FormArray>this.registerForm.get('lotes');
  }

  get redesSociais(): FormArray{
    return <FormArray>this.registerForm.get('redesSociais');
  }

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private fb: FormBuilder,
    private localeService: BsLocaleService,
    private toastr: ToastrService,
    private router: ActivatedRoute
    ) {
      this.localeService.use('pt-br'); // usado para formatar o datepicker dd/MM/yyyy
    }

    ngOnInit() {
      this.validation(); // validações do formulário
      this.carregarEvento();
    }

    carregarEvento(){
      const idEvento = +this.router.snapshot.paramMap.get('id');
      this.eventoService.getEventoById(idEvento)
        .subscribe(
          (evento: Evento) => {
            this.evento = Object.assign({}, evento); // Copia o evento passado por parâmetro para o evento já carregado para edição
            this.fileNameToUpdate = evento.imageUrl.toString();

            this.imagemURL = `http://localhost:5000/resources/images/${{this.evento.imageUrl}}?_v=${{this.dataAtual}}`;

            this.evento.imageUrl = '';
            this.registerForm.patchValue(this.evento);

            this.evento.lotes.forEach(lote => {
              this.lotes.push(this.criaLote(lote));
            });
            this.evento.redesSociais.forEach(redeSocial => {
              this.redesSociais.push(this.criaRedeSocial(redeSocial));
            });
          }
        );
    }

    validation(){
      this.registerForm = this.fb.group({
        id: [],
        tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
        local: ['', Validators.required],
        dataEvento: ['', Validators.required],
        qtdPessoas: ['', [Validators.required, Validators.max(500)]],
        imageUrl: [''],
        telefone: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        lotes: this.fb.array([]),
        redesSociais: this.fb.array([])
      });
    }

    criaLote(lote: any): FormGroup{
      return this.fb.group({
        id: [lote.id],
        nome: [lote.nome, Validators.required],
        quantidade: [lote.quantidade, Validators.required],
        preco: [lote.preco, Validators.required],
        dataInicio: [lote.dataInicio],
        dataFim: [lote.dataFim]
      });
    }

    criaRedeSocial(redeSocial: any): FormGroup{
      return this.fb.group({
        id: [redeSocial.id],
        nome: [redeSocial.nome, Validators.required],
        url: [redeSocial.url, Validators.required]
      });
    }

    adicionarLote(){
      this.lotes.push(this.criaLote({ id: 0}));
    }

    adicionarRedeSocial(){
      this.redesSociais.push(this.criaRedeSocial({ id: 0}));
    }

    removerLote(id: number){
      this.lotes.removeAt(id);
    }

    removerRedeSocial(id: number){
      this.redesSociais.removeAt(id);
    }

    onFileChange(file: FileList){
      const reader = new FileReader();

      reader.onload = (event: any) => this.imagemURL = event.target.result;

      this.file = event.target.files;

      reader.readAsDataURL(file[0]);
    }

    salvarEvento(){
      this.evento = Object.assign({id: this.evento.id}, this.registerForm.value);
      this.evento.imageUrl = this.fileNameToUpdate;

      this.uploadImagem();

      this.eventoService.putEvento(this.evento).subscribe(
        (novoEvento: Evento) => {
          console.log(novoEvento);

          this.toastr.success('Evento Atualizado com sucesso!');
        }, error => {
          this.toastr.error(`Não foi possível Editar o Evento: ${error}`);
          console.log(error);
        }
      );
    }

    uploadImagem(){
      if(this.registerForm.get('imagemURL').value !== ''){
        this.eventoService.postUpload(this.file, this.fileNameToUpdate).subscribe(
          () => {
            this.dataAtual = new Date().getMilliseconds().toString();
            this.imagemURL = `http://localhost:5000/resources/images/${{this.evento.imageUrl}}?_v=${{this.dataAtual}}`;
          }
        );
      }
    }

}
