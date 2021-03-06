import * as os from "os";
import * as path from "path";
import * as io from "./io";
import "./collection";
import {
  ConfigRepository,
  IConfigRepository,
} from "./config-repository";

const sourceDir = __dirname;
const pair = <T1, T2>(x1: T1, x2: T2): [T1, T2] => [x1, x2];

export interface IBatchContext {
  configRepo: IConfigRepository;
  printUsage(): void;
}

export type BatchCommand = {
  verb: string,
  alias?: string[] | undefined,
  help?: (() => string[]) | undefined,
  run(args: string[], context: IBatchContext): Promise<void>,
};

export class BatchApp {
  constructor(
    private readonly commands: BatchCommand[],
    private readonly commandFromVerb: Map<string, BatchCommand>,
    private readonly rootDir: string
  ) {
  }

  static create(commands: BatchCommand[]): BatchApp {
    const entries =
      commands.flatMap(c =>
        [c.verb, ...c.alias || []].map(verb => pair(c.verb, c))
      );
    const commandFromVerb = new Map(entries);

    const rootDir = path.join(sourceDir, "..");

    return new BatchApp(commands, commandFromVerb, rootDir);
  }

  usage() {
    const verbs =
      this.commands.map(c => c.verb);
    const helps =
      this.commands.choose(c => {
        if (c.help === undefined) return undefined;
        const help = c.help();
        if (help.length === 0) return undefined;
        return { command: c, help };
      });

    const lines = [
      "Verbs:",
      ...verbs.map(verb => `  ${verb}`),

      "Helps:",
      ...helps.flatMap(({ command, help }) => [
        `  ${command.verb}:`,
        ...help.map(line => `    ${line}`),
        "",
      ]),
    ];

    return lines;
  }

  printUsage() {
    console.info(this.usage().join(os.EOL));
  }

  async argumentList() {
    const [_0, _1, ...args] = process.argv;

    if (args.length === 0) {
      this.printUsage();
      console.log("Input argument:");
      const line = await io.readLine();
      return line.split(" ").filter(s => s !== "");
    }

    return args;
  }

  async execute() {
    const [verb, ...args] = await this.argumentList();

    const command = this.commandFromVerb.get(verb);
    if (command === undefined) {
      console.error(`Unknown verb '${verb}'.`);
      this.printUsage();
      return;
    }

    const context = new class implements IBatchContext {
      constructor(public readonly configRepo: IConfigRepository) {
      }

      printUsage(): void {
        this.printUsage();
      }
    }(ConfigRepository.create(this.rootDir));

    await command.run(args, context);
  }

  async main() {
    try {
      return await this.execute();
    } catch (ex) {
      console.error(ex);
      process.exit(1);
    }

    console.debug("End of main.");
  }
}
