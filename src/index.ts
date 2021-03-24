import * as core from "@actions/core"
import * as github from "@actions/github"
import { collect } from "./actions/collect"
import { merged } from "./actions/merged"
import { opened } from "./actions/opened"
import { readConfiguration } from "./configuration"
import { GithubPullRequest } from "./github"
import { GithubMediator } from "./mediators/github"

async function run() {
    const directory = core.getInput("directory", {
        required: false,
    })

    const configuration = await readConfiguration(directory).catch((reason) => {
        return Promise.reject(`Couldn't read configuration file.\n${reason}`)
    })

    const mediator = new GithubMediator(
        configuration,
        github.context.payload,
        directory,
    )

    if (
        core.getInput("collect", {
            required: false,
        }) === "true"
    ) {
        return collect(mediator)
    }

    const pullRequest = github.context.payload.pull_request as GithubPullRequest

    if (pullRequest === undefined) {
        return Promise.reject("No pull request detected.")
    }

    switch (github.context.payload.action) {
        case "opened":
            return opened(configuration, mediator, pullRequest, directory)
        case "closed":
            return merged(configuration, mediator, pullRequest, directory)
        default:
            core.info(`Unknown action: ${github.context.payload.action}`)
    }
}

run().catch((problem) => {
    core.setFailed(problem.toString())
})
